using Godot;
using System;
using IndustryMoudle;

namespace DirectorMoudle
{
    public partial class FactroyView : Control
    {
        // Called when the node enters the scene tree for the first time.
        private HBoxContainer? Storage;
        private VBoxContainer? Links;
        private Label? Recipe;
        private Label? Speed;

        public override void _Ready()
        {
            Recipe = GetNode<Label>("VBoxContainer/Recipe");
            Speed = GetNode<Label>("VBoxContainer/Speed");
            Storage = GetNode<HBoxContainer>("VBoxContainer/storage");
            Links = GetNode<VBoxContainer>("VBoxContainer/links");

        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public void Refresh(Factory factory)
        {
            Recipe!.Text = factory.Recipe;
            Speed!.Text = $"速度: {factory.BaseProduceSpeed.ToString()}";

            foreach (var child in Links!.GetChildren())
            {
                Links.RemoveChild(child);
            }
            foreach (var link in factory.InputLinks)
            {
                var laben = new Label();
                laben.Text = link + " in";
                Links.AddChild(laben);
            }
            foreach (var link in factory.OutputLinks)
            {
                var laben = new Label();
                laben.Text = link + " out";
                Links.AddChild(laben);
            }
        }
    }
}

