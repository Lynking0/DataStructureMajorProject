using Godot;
using System;
using Formula;

public partial class LWDTest : Node2D
{
    public override void _Ready()
    {
        // Test Code
        Program.MyFun(8000);
    }
}
