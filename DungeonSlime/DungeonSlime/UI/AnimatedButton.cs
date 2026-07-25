using System;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Gum.Graphics.Animation;
using Gum.Managers;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Adapters.MonoGame.Graphics;

namespace DungeonSlime.UI;

/// <summary>
/// A custom button implementation that inherits from Gum's Button class to provide
/// animated visual feedback when focused.
/// </summary>
internal class AnimatedButton : Button {
    /// <summary>
    /// Creates a new AnimatedButton instance using graphics from the specified texture atlas.
    /// </summary>
    /// <param name="atlas">The texture atlas containing button graphics and animations</param>
    public AnimatedButton(TextureAtlas atlas) {
        // Each Forms conrol has a general Visual property that
        // has properties shared by all control types. This Visual
        // type matches the Forms type. It can be casted to access
        // controls-specific properties.
        ButtonVisual visualButton = (ButtonVisual)Visual;
        // Width is relative to children with extra amountPadding, height is fixed
        visualButton.Height = 14f;
        visualButton.HeightUnits = DimensionUnitType.Absolute;
        visualButton.Width = 21f;
        visualButton.WidthUnits = DimensionUnitType.RelativeToChildren;

        // Get a reference to the nine-slice background to display the button graphics
        // A nine-slice allows the button to stretch while preserving corner appearance
        NineSliceRuntime runtimeBackground = visualButton.Background;
        runtimeBackground.Texture = atlas.Texture;
        runtimeBackground.TextureAddress = TextureAddress.Custom;
        runtimeBackground.Color = Microsoft.Xna.Framework.Color.White;
        // texture coordinates for the background are set by AnimationChains below

        TextRuntime runtimeTextInstance = visualButton.TextInstance;
        runtimeTextInstance.Text = "START";
        runtimeTextInstance.Blue = 130;
        runtimeTextInstance.Green = 86;
        runtimeTextInstance.Red = 70;
        runtimeTextInstance.UseCustomFont = true;
        runtimeTextInstance.CustomFontFile = "fonts/04b_30.fnt";
        runtimeTextInstance.FontScale = 0.25f;
        runtimeTextInstance.Anchor(Gum.Wireframe.Anchor.Center);
        runtimeTextInstance.Width = 0;
        runtimeTextInstance.WidthUnits = DimensionUnitType.RelativeToChildren;

        // Get the texture region for the unfocused button state from the atlas
        TextureRegion regionUnfocusedTexture = atlas.GetRegion("unfocused-button");

        // Create an animation chain for the unfocused state with a single frame
        AnimationChain chainUnfocusedAnimation = new AnimationChain();
        chainUnfocusedAnimation.Name = nameof(chainUnfocusedAnimation);
        AnimationFrame frameUnfocused = new AnimationFrame {
            TopCoordinate = regionUnfocusedTexture.TopTextureCoordinate,
            BottomCoordinate = regionUnfocusedTexture.BottomTextureCoordinate,
            LeftCoordinate = regionUnfocusedTexture.LeftTextureCoordinate,
            RightCoordinate = regionUnfocusedTexture.RightTextureCoordinate,
            FrameLength = 0.3f,
            Texture = regionUnfocusedTexture.Texture
        };
        chainUnfocusedAnimation.Add(frameUnfocused);

        // Get the multi-frame animation for the focused button state from the atlas
        Animation animationFocusedAtlas = atlas.GetAnimation("focused-button-animation");

        // Create an animation chain for the focused state using all frames from the atlas animation
        AnimationChain chainFocusedAnimation = new AnimationChain();
        chainFocusedAnimation.Name = nameof(chainFocusedAnimation);
        foreach (TextureRegion region in animationFocusedAtlas.Frames) {
            AnimationFrame frame = new AnimationFrame {
                TopCoordinate = region.TopTextureCoordinate,
                BottomCoordinate = region.BottomTextureCoordinate,
                LeftCoordinate = region.LeftTextureCoordinate,
                RightCoordinate = region.RightTextureCoordinate,
                FrameLength = (float)animationFocusedAtlas.Delay.TotalSeconds,
                Texture = region.Texture
            };
            
            chainFocusedAnimation.Add(frame);
        }
        
        // Assign both animation chains to the nine-slice background
        runtimeBackground.AnimationChains = new AnimationChainList {
            chainUnfocusedAnimation,
            chainFocusedAnimation
        };


        // Reset all state to default so we don't have unexpected variable assignments:
        visualButton.ButtonCategory.ResetAllStates();

        // Get the enabled (default/unfocused) state
        StateSave stateEnabled = visualButton.States.Enabled;
        stateEnabled.Apply = delegate() {
            // When enabled but not focused, use the unfocused animation
            runtimeBackground.CurrentChainName = chainUnfocusedAnimation.Name;
        };

        // Create the focused state
        StateSave stateFocused = visualButton.States.Focused;
        stateFocused.Apply = delegate() {
            // When focused, use the focused animation and enable animation playback
            runtimeBackground.CurrentChainName = chainFocusedAnimation.Name;
            runtimeBackground.Animate = true;
        };

        // Create the stateHighlighted+focused state (for mouse hover while focused)
        StateSave stateHighlightedAndFocused = visualButton.States.HighlightedFocused;
        stateHighlightedAndFocused.Apply = stateFocused.Apply;

        // Create the stateHighlighted state (for mouse hover)
        // by cloning the enabled state since they appear the same
        StateSave stateHighlighted = visualButton.States.Highlighted;
        stateHighlighted.Apply = stateEnabled.Apply;

        // Add event handlers for keyboard input.
        KeyDown += HandleKeyDown;

        // Add event handler for mouse hover focus.
        visualButton.RollOn += HandleRollOn;
    }

    /// <summary>
    /// Handles keyboard input for navigation between buttons using left/right keys.
    /// </summary>
    private void HandleKeyDown(object sender, KeyEventArgs arguments) {
        if (arguments.Key == Keys.Left) {
            // Left arrow navigates to previous control
            HandleTab(TabDirection.Up, loop: true);
        }
        if (arguments.Key == Keys.Right) {
            // Right arrow navigates to next control
            HandleTab(TabDirection.Down, loop: true);
        }
    }

    /// <summary>
    /// Automatically focuses the button when the mouse hovers over it.
    /// </summary>
    private void HandleRollOn(object sender, EventArgs arguments) {
        IsFocused = true;
    }
}
