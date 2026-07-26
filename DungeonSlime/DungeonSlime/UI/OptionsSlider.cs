using System;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Adapters.MonoGame.Graphics;

namespace DungeonSlime.UI;

/// <summary>
/// A custom slider control that inherits from Gum's Slider class.
/// </summary>
public class OptionsSlider : Slider {
    // Reference to the text label that displays the slider's title
    private TextRuntime _runtimeTextInstance;
    
    // Reference to the rectangle that visually represents the current value
    private ColoredRectangleRuntime _runtimeFillRectangle;
    
    /// <summary>
    /// Gets or sets the text label for this slider.
    /// </summary>
    public string Text {
        get { return _runtimeTextInstance.Text; }
        set { _runtimeTextInstance.Text = value; }
    }
    
    /// <summary>
    /// Creates a new OptionsSlider instance using graphics from the specified texture atlas.
    /// </summary>
    /// <param name="atlas">The texture atlas containing slider graphics.</param>
    public OptionsSlider(TextureAtlas atlas) {
        // Create the top-level container for all visual elements
        ContainerRuntime runtimeTopLevelContainer = new ContainerRuntime();
        runtimeTopLevelContainer.Height = 55f;
        runtimeTopLevelContainer.Width = 264f;
        
        TextureRegion regionBackground = atlas.GetRegion("panel-background");
        
        // Create the background panel that contains everything
        NineSliceRuntime runtimeBackground = new NineSliceRuntime();
        runtimeBackground.Texture = atlas.Texture;
        runtimeBackground.TextureAddress = TextureAddress.Custom;
        runtimeBackground.TextureHeight = regionBackground.Height;
        runtimeBackground.TextureLeft = regionBackground.SourceRectangle.Left;
        runtimeBackground.TextureTop = regionBackground.SourceRectangle.Top;
        runtimeBackground.TextureWidth = regionBackground.Width;
        runtimeBackground.Dock(Gum.Wireframe.Dock.Fill);
        runtimeTopLevelContainer.AddChild(runtimeBackground);
        
        // Create the title text element
        _runtimeTextInstance = new TextRuntime();
        _runtimeTextInstance.CustomFontFile = @"fonts/04b_30.fnt";
        _runtimeTextInstance.UseCustomFont = true;
        _runtimeTextInstance.FontScale = 0.5f;
        _runtimeTextInstance.Text = "Replace Me";
        _runtimeTextInstance.X = 10f;
        _runtimeTextInstance.Y = 10f;
        _runtimeTextInstance.WidthUnits = DimensionUnitType.RelativeToChildren;
        runtimeTopLevelContainer.AddChild(_runtimeTextInstance);
        
        // Create the container for the slider track and decorative elements
        ContainerRuntime runtimeInnerContainer = new ContainerRuntime();
        runtimeInnerContainer.Height = 13f;
        runtimeInnerContainer.Width = 241f;
        runtimeInnerContainer.X = 10f;
        runtimeInnerContainer.Y = 33f;
        runtimeTopLevelContainer.AddChild(runtimeInnerContainer);
        
        TextureRegion regionOffBackground = atlas.GetRegion("slider-off-background");
        
        // Create the "OFF" side of the slider (left end)
        NineSliceRuntime runtimeOffBackground = new NineSliceRuntime();
        runtimeOffBackground.Dock(Gum.Wireframe.Dock.Left);
        runtimeOffBackground.Texture = atlas.Texture;
        runtimeOffBackground.TextureAddress = TextureAddress.Custom;
        runtimeOffBackground.TextureHeight = regionOffBackground.Height;
        runtimeOffBackground.TextureLeft = regionOffBackground.SourceRectangle.Left;
        runtimeOffBackground.TextureTop = regionOffBackground.SourceRectangle.Top;
        runtimeOffBackground.TextureWidth = regionOffBackground.Width;
        runtimeOffBackground.Width = 28f;
        runtimeOffBackground.WidthUnits = DimensionUnitType.Absolute;
        runtimeInnerContainer.AddChild(runtimeOffBackground);
        
        TextureRegion regionMiddleBackground = atlas.GetRegion("slider-middle-background");
        
        // Create the middle track portion of the slider
        NineSliceRuntime runtimeMiddleBackground = new NineSliceRuntime();
        runtimeMiddleBackground.Texture = regionMiddleBackground.Texture;
        runtimeMiddleBackground.TextureAddress = TextureAddress.Custom;
        runtimeMiddleBackground.TextureHeight = regionMiddleBackground.Height;
        runtimeMiddleBackground.TextureLeft = regionMiddleBackground.SourceRectangle.Left;
        runtimeMiddleBackground.TextureTop = regionMiddleBackground.SourceRectangle.Top;
        runtimeMiddleBackground.TextureWidth = regionMiddleBackground.Width;
        runtimeMiddleBackground.Width = 179f;
        runtimeMiddleBackground.WidthUnits = DimensionUnitType.Absolute;
        runtimeMiddleBackground.Dock(Gum.Wireframe.Dock.Left);
        runtimeMiddleBackground.X = 27f;
        runtimeInnerContainer.AddChild(runtimeMiddleBackground);
        
        TextureRegion regionMaxBackground = atlas.GetRegion("slider-max-background");
        
        // Create the "MAX" side of the slider (right end)
        NineSliceRuntime runtimeMaxBackground = new NineSliceRuntime();
        runtimeMaxBackground.Texture = regionMaxBackground.Texture;
        runtimeMaxBackground.TextureAddress = TextureAddress.Custom;
        runtimeMaxBackground.TextureHeight = regionMaxBackground.Height;
        runtimeMaxBackground.TextureLeft = regionMaxBackground.SourceRectangle.Left;
        runtimeMaxBackground.TextureTop = regionMaxBackground.SourceRectangle.Top;
        runtimeMaxBackground.TextureWidth = regionMaxBackground.Width;
        runtimeMaxBackground.Width = 36f;
        runtimeMaxBackground.WidthUnits = DimensionUnitType.Absolute;
        runtimeMaxBackground.Dock(Gum.Wireframe.Dock.Right);
        runtimeInnerContainer.AddChild(runtimeMaxBackground);
        
        // Create the interactive track that responds to clicks
        // The special name "TrackInstance" is required for Slider functionality
        ContainerRuntime runtimeTrackInstance = new ContainerRuntime();
        runtimeTrackInstance.Name = "TrackInstance";
        runtimeTrackInstance.Dock(Gum.Wireframe.Dock.Fill);
        runtimeTrackInstance.Height = -2f;
        runtimeTrackInstance.Width = -2f;
        runtimeMiddleBackground.AddChild(runtimeTrackInstance);
        
        // Create the fill rectangle that visually displays the current value
        _runtimeFillRectangle = new ColoredRectangleRuntime();
        _runtimeFillRectangle.Dock(Gum.Wireframe.Dock.Left);
        _runtimeFillRectangle.Width = 90f; // Default to 90% - will be updated by value changes
        _runtimeFillRectangle.WidthUnits = DimensionUnitType.PercentageOfParent;
        runtimeTrackInstance.AddChild(_runtimeFillRectangle);
        
        // Add "OFF" text to the left end
        TextRuntime runtimeOffText = new TextRuntime();
        runtimeOffText.Red = 70;
        runtimeOffText.Green = 86;
        runtimeOffText.Blue = 130;
        runtimeOffText.CustomFontFile = @"fonts/04b_30.fnt";
        runtimeOffText.FontScale = 0.25f;
        runtimeOffText.UseCustomFont = true;
        runtimeOffText.Text = "OFF";
        runtimeOffText.Anchor(Gum.Wireframe.Anchor.Center);
        runtimeOffBackground.AddChild(runtimeOffText);
        
        // Add "MAX" text to the right end
        TextRuntime runtimeMaxText = new TextRuntime();
        runtimeMaxText.Red = 70;
        runtimeMaxText.Green = 86;
        runtimeMaxText.Blue = 130;
        runtimeMaxText.CustomFontFile = @"fonts/04b_30.fnt";
        runtimeMaxText.FontScale = 0.25f;
        runtimeMaxText.UseCustomFont = true;
        runtimeMaxText.Text = "MAX";
        runtimeMaxText.Anchor(Gum.Wireframe.Anchor.Center);
        runtimeMaxBackground.AddChild(runtimeMaxText);
        
        // Define colors for focused and unfocused states
        Color colorFocused = Color.White;
        Color colorUnfocused = Color.Gray;
        
        // Create slider state category - Slider.SliderCategoryName is the required name
        StateSaveCategory categorySlider = new StateSaveCategory();
        categorySlider.Name = Slider.SliderCategoryName;
        runtimeTopLevelContainer.AddCategory(categorySlider);
        
        // Create the enabled (default/unfocused) state
        StateSave stateEnabled = new StateSave();
        stateEnabled.Name = FrameworkElement.EnabledStateName;
        stateEnabled.Apply = delegate() {
            // When enabled but not focused, use gray coloring for all elements
            runtimeBackground.Color = colorUnfocused;
            _runtimeTextInstance.Color = colorUnfocused;
            runtimeOffBackground.Color = colorUnfocused;
            runtimeMiddleBackground.Color = colorUnfocused;
            runtimeMaxBackground.Color = colorUnfocused;
            _runtimeFillRectangle.Color = colorUnfocused;
        };
        categorySlider.States.Add(stateEnabled);
        
        // Create the focused state
        StateSave stateFocused = new StateSave();
        stateFocused.Name = FrameworkElement.FocusedStateName;
        stateFocused.Apply = delegate() {
            // When focused, use white coloring for all elements
            runtimeBackground.Color = colorFocused;
            _runtimeTextInstance.Color = colorFocused;
            runtimeOffBackground.Color = colorFocused;
            runtimeMiddleBackground.Color = colorFocused;
            runtimeMaxBackground.Color = colorFocused;
            _runtimeFillRectangle.Color = colorFocused;
        };
        categorySlider.States.Add(stateFocused);
        
        // Create the stateHighlighted+focused state by cloning the focused state
        StateSave stateHighlightedAndFocused = stateFocused.Clone();
        stateHighlightedAndFocused.Name = FrameworkElement.HighlightedFocusedStateName;
        categorySlider.States.Add(stateHighlightedAndFocused);
        
        // Create the stateHighlighted state by cloning the enabled state
        StateSave stateHighlighted = stateEnabled.Clone();
        stateHighlighted.Name = FrameworkElement.HighlightedStateName;
        categorySlider.States.Add(stateHighlighted);
        
        // Assign the configured container as this slider's visual
        Visual = runtimeTopLevelContainer;
        
        // Enable click-to-point functionality for the slider
        // This allows users to click anywhere on the track to jump to that value
        IsMoveToPointEnabled = true;
        
        // Add event handlers
        Visual.RollOn += HandleRollOn;
        ValueChanged += HandleValueChanged;
        ValueChangedByUi += HandleValueChangedByUi;
    }
    
    /// <summary>
    /// Automatically focuses the slider when the user interacts with it
    /// </summary>
    private void HandleValueChangedByUi(object sender, EventArgs arguments) {
        IsFocused = true;
    }
    
    /// <summary>
    /// Automatically focuses the slider when the mouse hovers over it
    /// </summary>
    private void HandleRollOn(object sender, EventArgs arguments) {
        IsFocused = true;
    }
    
    /// <summary>
    /// Updates the fill rectangle width to visually represent the current value
    /// </summary>
    private void HandleValueChanged(object sender, EventArgs arguments) {
        // Calculate the ratio of the current value within its range
        double ratio = (Value - Minimum) / (Maximum - Minimum);
        
        // Update the fill rectangle width as a percentage
        // _runtimeFillRectangle uses percentage width units, so we multiply by 100
        _runtimeFillRectangle.Width = 100 * (float)ratio;
    }
}
