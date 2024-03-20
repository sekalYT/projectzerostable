using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;

namespace UnityEditor.U2D.Animation
{
    [RequireSpriteDataProvider(typeof(ISpriteMeshDataProvider), typeof(ISpriteBoneDataProvider))]
    internal partial class SkinningModule : SpriteEditorModuleBase
    {
        private static class Styles
        {
            public static string moduleName = L10n.Tr("Skinning Editor");
        }

        private SkinningCache m_SkinningCache;
        private int m_PrevNearestControl = -1;
        private SpriteOutlineRenderer m_SpriteOutlineRenderer;
        private MeshPreviewTool m_MeshPreviewTool;
        private SkinningMode m_PreviousSkinningMode;
        private SpriteBoneInfluenceTool m_CharacterSpriteTool;
        private HorizontalToggleTools m_HorizontalToggleTools;
        private AnimationAnalytics m_Analytics;
        private ModuleToolGroup m_ModuleToolGroup;
        IMeshPreviewBehaviour m_MeshPreviewBehaviourOverride = null;
        bool m_CollapseToolbar;
        bool m_HasUnsavedChanges = false;
        Texture2D m_WorkspaceBackgroundTexture;

        internal SkinningCache skinningCache
        {
            get { return m_SkinningCache; }
        }

        private BaseTool currentTool
        {
            get { return skinningCache.selectedTool; }
            set { skinningCache.selectedTool = value; }
        }

        private BaseTool previousTool
        {
            get;
            set;
        }

        public override string moduleName
        {
            get { return Styles.moduleName; }
        }

        public override void OnModuleActivate()
        {
            m_SkinningCache = Cache.Create<SkinningCache>();
            m_WorkspaceBackgroundTexture = new Texture2D(1, 1, TextureFormat.RGBAHalf, false, true);
                        
            m_WorkspaceBackgroundTexture.hideFlags = HideFlags.HideAndDontSave;
            m_WorkspaceBackgroundTexture.SetPixel(1, 1, new Color(0, 0, 0, 0));
            m_WorkspaceBackgroundTexture.Apply();
            
            AddMainUI(spriteEditor.GetMainVisualContainer());

            using (skinningCache.DisableUndoScope())
            {
                skinningCache.Create(spriteEditor.GetDataProvider<ISpriteEditorDataProvider>(), SkinningCachePersistentState.instance);
                skinningCache.CreateToolCache(spriteEditor, m_LayoutOverlay);
                m_CharacterSpriteTool = skinningCache.CreateTool<SpriteBoneInfluenceTool>();
                m_CharacterSpriteTool.Initialize(m_LayoutOverlay);
                m_MeshPreviewTool = skinningCache.CreateTool<MeshPreviewTool>();
                SetupModuleToolGroup();
                m_MeshPreviewTool.Activate();

                m_SpriteOutlineRenderer = new SpriteOutlineRenderer(skinningCache.events);

                spriteEditor.enableMouseMoveEvent = true;
                EditorApplication.playModeStateChanged += PlayModeStateChanged;

                Undo.undoRedoPerformed += UndoRedoPerformed;
                skinningCache.events.skeletonTopologyChanged.AddListener(SkeletonTopologyChanged);
                skinningCache.events.skeletonPreviewPoseChanged.AddListener(SkeletonPreviewPoseChanged);
                skinningCache.events.skeletonBindPoseChanged.AddListener(SkeletonBindPoseChanged);
                skinningCache.events.characterPartChanged.AddListener(CharacterPartChanged);
                skinningCache.events.skinningModeChanged.AddListener(OnViewModeChanged);
                skinningCache.events.meshChanged.AddListener(OnMeshChanged);
                skinningCache.events.boneNameChanged.AddListener(OnBoneNameChanged);
                skinningCache.events.boneDepthChanged.AddListener(OnBoneDepthChanged);
                skinningCache.events.boneColorChanged.AddListener(OnBoneColorChanged);
                skinningCache.events.meshPreviewBehaviourChange.AddListener(OnMeshPreviewBehaviourChange);

                skinningCache.RestoreFromPersistentState();
                ActivateTool(skinningCache.selectedTool);
                skinningCache.RestoreToolStateFromPersistentState();

                // Set state for Switch Mode tool
                m_PreviousSkinningMode = skinningCache.mode;
                if (skinningCache.mode == SkinningMode.Character)
                {
                    skinningCache.GetTool(Tools.SwitchMode).Deactivate();
                }
                else
                {
                    skinningCache.GetTool(Tools.SwitchMode).Activate();
                }
                SetupSpriteEditor(true);

                m_HorizontalToggleTools = new HorizontalToggleTools(skinningCache)
                {
                    onActivateTool = (b) =>
                    {
                        using (skinningCache.UndoScope(TextContent.setTool))
                        {
                            ActivateTool(b);
                        }
                    }
                };

                var ai = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>() as AssetImporter;
                m_Analytics = new AnimationAnalytics(new UnityAnalyticsStorage(),
                    skinningCache.events,
                    new SkinningModuleAnalyticsModel(skinningCache),
                    ai ==  null ? -1 : ai.GetInstanceID());

                UpdateCollapseToolbar();
            }
        }    
  
        public override void OnModuleDeactivate()
        {
            if (m_SpriteOutlineRenderer != null)
                m_SpriteOutlineRenderer.Dispose();

            spriteEditor.enableMouseMoveEvent = false;
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;

            Undo.undoRedoPerformed -= UndoRedoPerformed;
            skinningCache.events.skeletonTopologyChanged.RemoveListener(SkeletonTopologyChanged);
            skinningCache.events.skeletonPreviewPoseChanged.RemoveListener(SkeletonPreviewPoseChanged);
            skinningCache.events.skeletonBindPoseChanged.RemoveListener(SkeletonBindPoseChanged);
            skinningCache.events.characterPartChanged.RemoveListener(CharacterPartChanged);
            skinningCache.events.skinningModeChanged.RemoveListener(OnViewModeChanged);
            skinningCache.events.meshChanged.RemoveListener(OnMeshChanged);
            skinningCache.events.boneNameChanged.RemoveListener(OnBoneNameChanged);
            skinningCache.events.boneDepthChanged.RemoveListener(OnBoneDepthChanged);
            skinningCache.events.boneColorChanged.RemoveListener(OnBoneColorChanged);
            skinningCache.events.meshPreviewBehaviourChange.RemoveListener(OnMeshPreviewBehaviourChange);

            RemoveMainUI(spriteEditor.GetMainVisualContainer());
            RestoreSpriteEditor();
            m_Analytics.Dispose();
            m_Analytics = null;
            
            Cache.Destroy(m_SkinningCache);
        }
        
        void PlayModeStateChanged(PlayModeStateChange newState)
        {
            if (newState == PlayModeStateChange.ExitingEditMode && m_HasUnsavedChanges)
            {
                var shouldApply = EditorUtility.DisplayDialog(TextContent.savePopupTitle, TextContent.savePopupMessage, TextContent.savePopupOptionYes, TextContent.savePopupOptionNo);
                spriteEditor.ApplyOrRevertModification(shouldApply);
            }
        }        

        private void UpdateCollapseToolbar()
        {
            m_CollapseToolbar = SkinningModuleSettings.compactToolBar;
            m_PoseToolbar.CollapseToolBar(m_CollapseToolbar);
            m_WeightToolbar.CollapseToolBar(m_CollapseToolbar);
            m_MeshToolbar.CollapseToolBar(m_CollapseToolbar);
            m_BoneToolbar.CollapseToolBar(m_CollapseToolbar);
            m_RigToolbar.CollapseToolBar(m_CollapseToolbar);
            m_LayoutOverlay.verticalToolbar.Collapse(m_CollapseToolbar);
            m_HorizontalToggleTools.collapseToolbar = m_CollapseToolbar;
        }

        private void OnBoneNameChanged(BoneCache bone)
        {
            var character = skinningCache.character;

            if (character != null && character.skeleton == bone.skeleton)
                skinningCache.SyncSpriteSheetSkeletons();
            DataModified();
        }

        private void OnBoneDepthChanged(BoneCache bone)
        {
            var sprites = skinningCache.GetSprites();
            var controller = new SpriteMeshDataController();

            foreach (var sprite in sprites)
            {
                var mesh = sprite.GetMesh();

                if (mesh.ContainsBone(bone))
                {
                    controller.spriteMeshData = mesh;
                    controller.SortTrianglesByDepth();
                    skinningCache.events.meshChanged.Invoke(mesh);
                }
            }

            DataModified();
        }

        private void OnBoneColorChanged(BoneCache bone)
        {
            DataModified();            
        }
        
        private void OnMeshChanged(MeshCache mesh)
        {
            DataModified();
        }

        private void DataModified()
        {
            spriteEditor.SetDataModified();
            m_HasUnsavedChanges = true;
        }

        private void OnViewModeChanged(SkinningMode mode)
        {
            SetupSpriteEditor();
        }

        private void SetupSpriteEditor(bool setPreviewTexture = false)
        {
            var textureProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();
            if (textureProvider == null)
                return;

            int width = 0, height = 0;
            if (skinningCache.mode == SkinningMode.SpriteSheet)
            {
                textureProvider.GetTextureActualWidthAndHeight(out width, out height);
            }
            else
            {
                width = skinningCache.character.dimension.x;
                height = skinningCache.character.dimension.y;
            }

            if (m_PreviousSkinningMode != skinningCache.mode || setPreviewTexture)
            {
                spriteEditor.SetPreviewTexture(m_WorkspaceBackgroundTexture, width, height);
                if (m_PreviousSkinningMode != skinningCache.mode)
                {
                    m_PreviousSkinningMode = skinningCache.mode;
                    spriteEditor.ResetZoomAndScroll();
                }
            }

            spriteEditor.spriteRects = new List<SpriteRect>();
        }

        private void RestoreSpriteEditor()
        {
            var textureProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();

            if (textureProvider != null)
            {
                int width, height;
                textureProvider.GetTextureActualWidthAndHeight(out width, out height);

                var texture = textureProvider.previewTexture;
                spriteEditor.SetPreviewTexture(texture, width, height);
            }

            var spriteRectProvider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();

            if (spriteRectProvider != null)
                spriteEditor.spriteRects = new List<SpriteRect>(spriteRectProvider.GetSpriteRects());
        }

        public override bool CanBeActivated()
        {
            var dataProvider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();
            return dataProvider == null ? false : dataProvider.spriteImportMode != SpriteImportMode.None;
        }

        public override void DoPostGUI()
        {
            if (!spriteEditor.windowDimension.Contains(Event.current.mousePosition))
                HandleUtility.nearestControl = 0;

            if (Event.current.type == EventType.Layout && m_PrevNearestControl != HandleUtility.nearestControl)
            {
                m_PrevNearestControl = HandleUtility.nearestControl;
                spriteEditor.RequestRepaint();
            }

            skinningCache.EndUndoOperation();
        }

        public override void DoMainGUI()
        {
            Debug.Assert(currentTool != null);

            DoViewGUI();

            if (!spriteEditor.editingDisabled)
                skinningCache.selectionTool.DoGUI();

            m_MeshPreviewTool.previewBehaviourOverride = m_MeshPreviewBehaviourOverride != null ? m_MeshPreviewBehaviourOverride : currentTool.previewBehaviour;
            m_MeshPreviewTool.DoGUI();
            m_MeshPreviewTool.DrawOverlay();

            if(Event.current.type == EventType.Repaint)
                m_SpriteOutlineRenderer.RenderSpriteOutline(spriteEditor, skinningCache.selectedSprite);

            m_MeshPreviewTool.OverlayWireframe();

            if (!spriteEditor.editingDisabled)
            {
                currentTool.DoGUI();
                DoCopyPasteKeyboardEventHandling();
            }

            DrawRectGizmos();

            if (SkinningModuleSettings.compactToolBar != m_CollapseToolbar)
                UpdateCollapseToolbar();
        }

        public override void DoToolbarGUI(Rect drawArea)
        {
            m_HorizontalToggleTools.DoGUI(drawArea, currentTool, spriteEditor.editingDisabled);
        }

        void DoCopyPasteKeyboardEventHandling()
        {
            var evt = Event.current;
            if (evt.type == EventType.ValidateCommand)
            {
                if (evt.commandName == "Copy" || evt.commandName == "Paste")
                    evt.Use();
                return;
            }

            if (evt.type == EventType.ExecuteCommand)
            {
                var copyTool = skinningCache.GetTool(Tools.CopyPaste) as CopyTool;
                if (copyTool != null && evt.commandName == "Copy")
                {
                    copyTool.OnCopyActivated();
                    evt.Use();
                }
                else if (copyTool != null && evt.commandName == "Paste")
                {
                    var boneReadOnly = skinningCache.bonesReadOnly;
                    copyTool.OnPasteActivated(!boneReadOnly, true, false, false);
                    evt.Use();
                }
            }
        }

        private void DrawRectGizmos()
        {
            if (Event.current.type == EventType.Repaint)
            {
                var selectedSprite = skinningCache.selectedSprite;
                var sprites = skinningCache.GetSprites();
                var unselectedRectColor = new Color(1f, 1f, 1f, 0.5f);

                foreach (var sprite in sprites)
                {
                    var skeleton = skinningCache.GetEffectiveSkeleton(sprite);

                    Debug.Assert(skeleton != null);

                    if (skeleton.isPosePreview)
                        continue;

                    var color = unselectedRectColor;

                    if (sprite == selectedSprite)
                        color = DrawingUtility.kSpriteBorderColor;

                    if (skinningCache.mode == SkinningMode.Character
                        && sprite != selectedSprite)
                        continue;

                    var matrix = sprite.GetLocalToWorldMatrixFromMode();
                    var rect = new Rect(matrix.MultiplyPoint3x4(Vector3.zero), sprite.textureRect.size);

                    DrawingUtility.BeginLines(color);
                    DrawingUtility.DrawBox(rect);
                    DrawingUtility.EndLines();
                }
            }
        }

        private void UndoRedoPerformed()
        {
            using (new DisableUndoScope(skinningCache))
            {
                UpdateToggleState();
                skinningCache.UndoRedoPerformed();
                SetupSpriteEditor();
            }
        }

        #region CharacterConsistency
        //TODO: Bring this to a better place, maybe CharacterController
        private void SkeletonPreviewPoseChanged(SkeletonCache skeleton)
        {
            var character = skinningCache.character;

            if (character != null && character.skeleton == skeleton)
                skinningCache.SyncSpriteSheetSkeletons();
        }

        private void SkeletonBindPoseChanged(SkeletonCache skeleton)
        {
            var character = skinningCache.character;

            if (character != null && character.skeleton == skeleton)
                skinningCache.SyncSpriteSheetSkeletons();
            DataModified();
        }

        private void SkeletonTopologyChanged(SkeletonCache skeleton)
        {
            var character = skinningCache.character;

            if (character == null)
            {
                var sprite = FindSpriteFromSkeleton(skeleton);

                Debug.Assert(sprite != null);

                sprite.UpdateMesh(skeleton.bones);

                DataModified();
            }
            else if (character.skeleton == skeleton)
            {
                skinningCache.CreateSpriteSheetSkeletons();
                DataModified();
            }
        }

        private void CharacterPartChanged(CharacterPartCache characterPart)
        {
            var character = skinningCache.character;

            Debug.Assert(character != null);

            using (new DefaultPoseScope(character.skeleton))
            {
                skinningCache.CreateSpriteSheetSkeleton(characterPart);
                DataModified();
            }

            if (skinningCache.mode == SkinningMode.Character)
                characterPart.SyncSpriteSheetSkeleton();
        }

        private SpriteCache FindSpriteFromSkeleton(SkeletonCache skeleton)
        {
            var sprites = skinningCache.GetSprites();
            return sprites.FirstOrDefault(sprite => sprite.GetSkeleton() == skeleton);
        }

        #endregion

        public override bool ApplyRevert(bool apply)
        {
            if (apply)
            {
                m_Analytics.FlushEvent();
                ApplyChanges(skinningCache, spriteEditor.GetDataProvider<ISpriteEditorDataProvider>());
                DoApplyAnalytics();
            }
            else
                skinningCache.Revert();

            m_HasUnsavedChanges = false;
            return true;
        }

        internal static void ApplyChanges(SkinningCache skinningCache, ISpriteEditorDataProvider dataProvider)
        {
            skinningCache.applyingChanges = true;
            skinningCache.RestoreBindPose();
            ApplyBone(skinningCache, dataProvider);
            ApplyMesh(skinningCache, dataProvider);
            ApplyCharacter(skinningCache, dataProvider);
            skinningCache.applyingChanges = false;
        }
        
        private void DoApplyAnalytics()
        {
            var sprites = skinningCache.GetSprites();
            var spriteBoneCount = sprites.Select(s => s.GetSkeleton().boneCount).ToArray();
            BoneCache[] bones = null;

            if (skinningCache.hasCharacter)
                bones = skinningCache.character.skeleton.bones;
            else
                bones = sprites.SelectMany(s => s.GetSkeleton().bones).ToArray();

            m_Analytics.SendApplyEvent(sprites.Length, spriteBoneCount, bones);
        }

        static void ApplyBone(SkinningCache skinningCache, ISpriteEditorDataProvider dataProvider)
        {
            var boneDataProvider = dataProvider.GetDataProvider<ISpriteBoneDataProvider>();
            if (boneDataProvider != null)
            {
                var sprites = skinningCache.GetSprites();
                foreach (var sprite in sprites)
                {
                    var bones = sprite.GetSkeleton().bones;
                    boneDataProvider.SetBones(new GUID(sprite.id), bones.ToSpriteBone(sprite.localToWorldMatrix).ToList());
                }
            }
        }

        static void ApplyMesh(SkinningCache skinningCache, ISpriteEditorDataProvider dataProvider)
        {
            var meshDataProvider = dataProvider.GetDataProvider<ISpriteMeshDataProvider>();
            if (meshDataProvider != null)
            {
                var sprites = skinningCache.GetSprites();
                foreach (var sprite in sprites)
                {
                    var mesh = sprite.GetMesh();
                    var guid = new GUID(sprite.id);

                    var vertices = new Vertex2DMetaData[mesh.vertexCount];
                    for (var i = 0; i < vertices.Length; ++i)
                    {
                        vertices[i].position = mesh.vertices[i];
                        vertices[i].boneWeight = mesh.vertexWeights[i].ToBoneWeight(false);
                    }

                    meshDataProvider.SetVertices(guid, vertices);
                    meshDataProvider.SetIndices(guid, mesh.indices);

                    var edgeVectArr = EditorUtilities.ToVector2Int(mesh.edges);
                    meshDataProvider.SetEdges(guid, edgeVectArr);
                }
            }
        }

        static void ApplyCharacter(SkinningCache skinningCache, ISpriteEditorDataProvider dataProvider)
        {
            var characterDataProvider = dataProvider.GetDataProvider<ICharacterDataProvider>();
            var character = skinningCache.character;
            if (characterDataProvider != null && character != null)
            {
                var data = new CharacterData();
                var characterBones = character.skeleton.bones;
                data.bones = characterBones.ToSpriteBone(Matrix4x4.identity);

                var parts = character.parts;
                data.parts = parts.Select(x =>
                    new CharacterPart()
                    {
                        spriteId = x.sprite.id,
                        spritePosition = new RectInt((int)x.position.x, (int)x.position.y, (int)x.sprite.textureRect.width, (int)x.sprite.textureRect.height),
                        bones = x.bones.Select(bone => Array.IndexOf(characterBones, bone)).ToArray()
                    }
                    ).ToArray();

                characterDataProvider.SetCharacterData(data);
                
            }
        }
        
        void OnMeshPreviewBehaviourChange(IMeshPreviewBehaviour meshPreviewBehaviour)
        {
            m_MeshPreviewBehaviourOverride = meshPreviewBehaviour;
        }

        private void SetupModuleToolGroup()
        {
            m_ModuleToolGroup = new ModuleToolGroup();
            m_ModuleToolGroup.AddToolToGroup(0, skinningCache.GetTool(Tools.Visibility), null);
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.EditGeometry), () => currentTool = skinningCache.GetTool(Tools.EditGeometry));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.CreateVertex), () => currentTool = skinningCache.GetTool(Tools.CreateVertex));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.CreateEdge), () => currentTool = skinningCache.GetTool(Tools.CreateEdge));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.SplitEdge), () => currentTool = skinningCache.GetTool(Tools.SplitEdge));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.GenerateGeometry), () => currentTool = skinningCache.GetTool(Tools.GenerateGeometry));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.EditPose), () => currentTool = skinningCache.GetTool(Tools.EditPose));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.EditJoints), () => currentTool = skinningCache.GetTool(Tools.EditJoints));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.CreateBone), () => currentTool = skinningCache.GetTool(Tools.CreateBone));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.SplitBone), () => currentTool = skinningCache.GetTool(Tools.SplitBone));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.WeightSlider), () => currentTool = skinningCache.GetTool(Tools.WeightSlider));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.WeightBrush), () => currentTool = skinningCache.GetTool(Tools.WeightBrush));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.GenerateWeights), () => currentTool = skinningCache.GetTool(Tools.GenerateWeights));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.BoneInfluence), () => currentTool = skinningCache.GetTool(Tools.BoneInfluence));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.SpriteInfluence), () => currentTool = skinningCache.GetTool(Tools.SpriteInfluence));
            m_ModuleToolGroup.AddToolToGroup(1, skinningCache.GetTool(Tools.CopyPaste), () => currentTool = skinningCache.GetTool(Tools.CopyPaste));
        }
    }
}

