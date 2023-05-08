using Godot;
using System.Linq;
using IndustryMoudle;
using IndustryMoudle.Extensions;
using TransportMoudle;

namespace DirectorMoudle
{

    public partial class TrainLineView : PanelContainer
    {
        private VBoxContainer? Factories;
        private Label? NameLabel;
        private Label? MaxLoad;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Factories = GetNode<VBoxContainer>("VBoxContainer/VBoxContainer/Factories");
            NameLabel = GetNode<Label>("VBoxContainer/VBoxContainer/Name");
            MaxLoad = GetNode<Label>("VBoxContainer/VBoxContainer/MaxLoad");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public void Refresh(TrainLine line)
        {
            NameLabel!.Text = $"{line.ID}";
            NameLabel!.LabelSettings.FontColor = line.Color;
            // MaxLoad!.Text = $"最大负载: {line.MaxLoad.ToString()}";

            foreach (var child in Factories!.GetChildren())
            {
                Factories.RemoveChild(child);
            }


            foreach (var factory in line.Vertexes.Select(v => v.GetFactory()))
            {
                var label = new Label();
                label.Text = $"工厂 {factory!.ID}";
                label.MouseFilter = MouseFilterEnum.Stop;
                label.GuiInput += e => OnFactoryClick(e, label);
                Factories.AddChild(label);
            }
        }
        public void OnFactoryClick(InputEvent @event, Label target)
        {
            if (@event is InputEventMouse mouseEvent)
            {
                if (mouseEvent.IsPressed())
                {
                    var window = GetNode<Window>("/root/Main/FactroyViewWindow");
                    window.Popup();
                    var id = System.Text.RegularExpressions.Regex.Replace(target.Text, @"[^0-9]+", "").ToInt();
                    window.GetNode<FactroyView>("FactroyView").Refresh(
                        Factory.Factories.Where(f => f.ID == id).First());
                }
            }
        }
    }

}