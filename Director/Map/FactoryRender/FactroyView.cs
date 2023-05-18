using Godot;
using System.Linq;
using IndustryMoudle;
using TransportMoudle;
using TransportMoudle.Extensions;
namespace DirectorMoudle
{
    public partial class FactroyView : Control
    {
        public static FactroyView? Instance;
        // Called when the node enters the scene tree for the first time.
        private HBoxContainer? Storage;
        private VBoxContainer? Links;
        private VBoxContainer? TrainLines;
        private VBoxContainer? Goodses;
        private Label? NameLabel;
        private Label? Recipe;
        private Label? MaximumCapacity;
        private Label? AvailableCapacity;
        private Label? ProduceCount;

        Factory? Factory;

        public FactroyView()
        {
            Instance = this;
        }

        public override void _Ready()
        {
            var basePath = "ScrollContainer/VBoxContainer";
            NameLabel = GetNode<Label>($"{basePath}/Name");
            Recipe = GetNode<Label>($"{basePath}/Recipe");
            MaximumCapacity = GetNode<Label>($"{basePath}/MaximumCapacity");
            AvailableCapacity = GetNode<Label>($"{basePath}/AvailableCapacity");
            ProduceCount = GetNode<Label>($"{basePath}/ProduceCount");
            Storage = GetNode<HBoxContainer>($"{basePath}/storage");
            Links = GetNode<VBoxContainer>($"{basePath}/links");
            TrainLines = GetNode<VBoxContainer>($"{basePath}/TrainLines");
            Goodses = GetNode<VBoxContainer>($"{basePath}/Goodses");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
        public void Refresh(Factory factory)
        {
            Factory = factory;
            Refresh();
        }
        public void Refresh()
        {
            if (Factory is null)
            {
                return;
            }
            NameLabel!.Text = $"工厂: {Factory.ID}";
            Recipe!.Text = Factory.Recipe;
            MaximumCapacity!.Text = $"最大产能: {Factory.BaseProduceSpeed.ToString()}";
            AvailableCapacity!.Text = $"可用产能: {((Factory.IdealOutput.ToList().Count > 0) ? Factory.IdealOutput.ToList()[0].number.ToString() : "0")}";
            ProduceCount!.Text = $"总生产: {Factory.ProduceCount}";
            foreach (var child in Links!.GetChildren())
            {
                Links.RemoveChild(child);
            }
            foreach (var child in TrainLines!.GetChildren())
            {
                TrainLines.RemoveChild(child);
            }
            foreach (var child in Storage!.GetChildren())
            {
                Storage.RemoveChild(child);
            }
            foreach (var child in Goodses!.GetChildren())
            {
                Goodses.RemoveChild(child);
            }
            foreach (var link in Factory.InputLinks)
            {
                var label = new Label();
                label.Text = $"{link.ID} {(string)link} in from {link.From.ID} for {link.For?.ID ?? -1}";
                Links.AddChild(label);
            }
            foreach (var link in Factory.OutputLinks)
            {
                var label = new Label();
                label.Text = $"{link.ID} {(string)link} out to {link.To.ID} for {link.For?.ID ?? -1}";
                Links.AddChild(label);
            }
            foreach (var goods in Factory.Platform.SelectMany(a => a.Value)
                .GroupBy(a => a.Item.Type))
            {
                var label = new Label();

                label.Text = $"{(string)goods.Key} x {goods.Sum(g => g.Item.Number)}";
                Goodses.AddChild(label);
            }
            foreach (var item in Factory.Storage)
            {
                var c = new VBoxContainer();
                var name = new Label();
                name.Text = item.Key;
                var number = new Label();
                number.Text = item.Value.ToString();
                c.AddChild(name);
                c.AddChild(number);
                Storage.AddChild(c);
            }
            foreach (var line in Factory.Vertex.GetTrainLines())
            {
                var label = new Label();
                label.Text = $"{line.ID}";
                label.LabelSettings = new LabelSettings();
                label.LabelSettings.FontColor = line.Color;
                switch (line.Level)
                {
                    case TrainLineLevel.MainLine:
                        label.LabelSettings.FontSize = 24;
                        break;
                    case TrainLineLevel.SideLine:
                        label.LabelSettings.FontSize = 20;
                        break;
                    case TrainLineLevel.FootPath:
                        label.LabelSettings.FontSize = 16;
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

