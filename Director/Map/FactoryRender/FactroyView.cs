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
        private Label? MaximumCapacity;
        private Label? AvailableCapacity;

        public override void _Ready()
        {
            Recipe = GetNode<Label>("VBoxContainer/Recipe");
            MaximumCapacity = GetNode<Label>("VBoxContainer/MaximumCapacity");
            AvailableCapacity = GetNode<Label>("VBoxContainer/AvailableCapacity");
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
            MaximumCapacity!.Text = $"最大产能: {factory.BaseProduceSpeed.ToString()}";
            AvailableCapacity!.Text = $"可用产能: {((factory.IdealOutput.ToList().Count > 0) ? factory.IdealOutput.ToList()[0].number.ToString() : "0")}";

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

