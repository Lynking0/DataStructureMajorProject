using Godot;
using System.Linq;
using IndustryMoudle;
using TransportMoudle;
using TransportMoudle.Extensions;
namespace DirectorMoudle
{
    public partial class FactroyView : Control
    {
        // Called when the node enters the scene tree for the first time.
        private HBoxContainer? Storage;
        private VBoxContainer? Links;
        private VBoxContainer? TrainLines;
        private Label? NameLabel;
        private Label? Recipe;
        private Label? MaximumCapacity;
        private Label? AvailableCapacity;

        public override void _Ready()
        {
            var basePath = "ScrollContainer/VBoxContainer";
            NameLabel = GetNode<Label>($"{basePath}/Name");
            Recipe = GetNode<Label>($"{basePath}/Recipe");
            MaximumCapacity = GetNode<Label>($"{basePath}/MaximumCapacity");
            AvailableCapacity = GetNode<Label>($"{basePath}/AvailableCapacity");
            Storage = GetNode<HBoxContainer>($"{basePath}/storage");
            Links = GetNode<VBoxContainer>($"{basePath}/links");
            TrainLines = GetNode<VBoxContainer>($"{basePath}/TrainLines");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public void Refresh(Factory factory)
        {
            NameLabel!.Text = $"工厂: {factory.ID}";
            Recipe!.Text = factory.Recipe;
            MaximumCapacity!.Text = $"最大产能: {factory.BaseProduceSpeed.ToString()}";
            AvailableCapacity!.Text = $"可用产能: {((factory.IdealOutput.ToList().Count > 0) ? factory.IdealOutput.ToList()[0].number.ToString() : "0")}";

            foreach (var child in Links!.GetChildren())
            {
                Links.RemoveChild(child);
            }
            foreach (var child in TrainLines!.GetChildren())
            {
                TrainLines.RemoveChild(child);
            }
            foreach (var link in factory.InputLinks)
            {
                var label = new Label();
                label.Text = $"{(string)link} in from {link.From.ID}";
                Links.AddChild(label);
            }
            foreach (var link in factory.OutputLinks)
            {
                var label = new Label();
                label.Text = $"{(string)link} out to {link.To.ID}";
                Links.AddChild(label);
            }

            foreach (var line in factory.Vertex.GetTrainLines())
            {
                var label = new Label();
                label.Text = $"{line.ID}";
                label.LabelSettings = new LabelSettings();
                label.LabelSettings.FontColor = line.Color;
                switch (line.Level)
                {
                    case TrainLineLevel.MainLine:
                        label.LabelSettings.FontSize = 20;
                        break;
                    case TrainLineLevel.SideLine:
                        label.LabelSettings.FontSize = 16;
                        break;
                    case TrainLineLevel.FootPath:
                        label.LabelSettings.FontSize = 12;
                        break;
                }
                label.LabelSettings.OutlineSize = 4;
                label.LabelSettings.OutlineColor = Colors.White;
                label.MouseFilter = MouseFilterEnum.Stop;
                label.GuiInput += e => OnTrainLineClick(e, label);
                TrainLines.AddChild(label);
            }
        }

        public void OnTrainLineClick(InputEvent @event, Label target)
        {
            if (@event is InputEventMouse mouseEvent)
            {
                if (mouseEvent.IsPressed())
                {
                    var window = GetNode<Window>("/root/Main/TrainLineViewWindow");
                    window.Popup();
                    window.GetNode<TrainLineView>("TrainLineView").Refresh(TrainLine.TrainLines.Where(t => t.ID == target.Text).First());
                }
            }
        }
    }
}

