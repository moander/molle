﻿using Boo.Lang;
using System.Linq;
using System;
using System.Diagnostics;

internal class MyContext
{
    public static readonly MyContext Current = new MyContext();
    public readonly DotBehaviour[] Dots = new DotBehaviour[24];
    public bool IsCurrentPlayerWhite;
    public int DotsLeftToPlay;

    public int DotsLeftBlack;
    public int DotsLeftWhite;
    public bool GameOver;

    internal void RegisterDot(int dotIndex, DotBehaviour dot)
    {
        Dots[dotIndex] = dot;

        if (!Dots.Any(p => p == null))
        {
            ResetGame();
        }
    }

    internal void ResetGame()
    {
        IsCurrentPlayerWhite = true;
        DotsLeftToPlay = 8;

        DotsLeftBlack = 0;
        DotsLeftWhite = 0;
        GameOver = false;

        foreach (var dot in Dots)
        {
            dot.ResetDot();
        }

    }

    internal DotBehaviour CurrentlySelectedDot;

    internal ColorIndicatorBehaviour ColorIndicator;

    public void UpdateDots(DotBehaviour currentDot, bool DoSwitchPlayer)
    {
        var oldHasThreeInRow = currentDot.HasThreeInRow;

        foreach (var dot in Dots)
        {
            dot.HasThreeInRow = false;
        }

        for (var i = 1; i < Dots.Length; i += 2)
        {
            var dot = Dots[i];

            var CircleIndex = dot.DotIndex % 8;

            if (CircleIndex == 7)
            {
                UpdateAndSetThreeInRow(dot, -7, -1);
            }
            else
            {
                UpdateAndSetThreeInRow(dot, -1, 1);
            }
        }

        for (var i = 9; i <= 15; i += 2)
        {
            var dot = Dots[i];

            UpdateAndSetThreeInRow(dot, -8, 8);
        }

        if (DoSwitchPlayer && !oldHasThreeInRow && currentDot.HasThreeInRow)
        {
            foreach (var dot in Dots)
            {
                dot.MyCanSelect = dot.CurrentMode == DotBehaviour.Mode.IsPlayer &&
                                  dot.isWhite != currentDot.isWhite &&
                                  !dot.HasThreeInRow;
            }

            return;
        }

        if (DoSwitchPlayer)
        {
            IsCurrentPlayerWhite = !IsCurrentPlayerWhite;
            CurrentlySelectedDot = null;
            foreach (var dot in Dots)
            {
                dot.CurrentScaleSin = 0.0F;
            }
        }


        var DotsLeftCurrentPlayer = IsCurrentPlayerWhite ? DotsLeftWhite : DotsLeftBlack;

        if (DotsLeftToPlay > 0)
        {
            // Put new anywhere free
            foreach (var dot in Dots)
            {
                dot.MyCanSelect = dot.CurrentMode == DotBehaviour.Mode.NotUsed;
            }
        }
        else if (CurrentlySelectedDot == null)
        {
            // Select dot to move
            foreach (var dot in Dots)
            {
                dot.MyCanSelect = dot.IsCurrentColor;
            }

            // Second select dot to move check
            foreach (var dot in Dots.Where(p => p.MyCanSelect))
            {
                dot.MyCanSelect = dot.canTouch.Select(p => this.Dots[p])
                    .Any(p => p.CurrentMode == DotBehaviour.Mode.NotUsed);

            }
        }
        else
        {
            // Select where to place selected dot
            if (DotsLeftCurrentPlayer > 3)
            {
                // Select dot to move
                foreach (var dot in Dots)
                {
                    dot.MyCanSelect = false;
                }

                foreach (var i in CurrentlySelectedDot.canTouch)
                {
                    var dot = Dots[i];
                    dot.MyCanSelect = dot.CurrentMode == DotBehaviour.Mode.NotUsed;
                }
            }
            else if (DotsLeftCurrentPlayer == 3)
            {
                // Select dot to move
                foreach (var dot in Dots)
                {
                    dot.MyCanSelect = dot.CurrentMode == DotBehaviour.Mode.NotUsed;
                }
            }
        }

        // Check if no moves is available
        if (!Dots.Any(p => p.MyCanSelect))
        {
            if (++noPossibleMoveCounter == 1)
            {
                UpdateDots(currentDot, true);
            }
            else if (noPossibleMoveCounter > 1)
            {
                SetGameOver();
            }
        }
        else
        {
            noPossibleMoveCounter = 0;
        }

        // Check if game over
        if (DotsLeftToPlay == 0 && (DotsLeftWhite < 3 || DotsLeftBlack < 3))
        {
            SetGameOver();
        }
    }

    int noPossibleMoveCounter = 0;

    void UpdateAndSetThreeInRow(DotBehaviour dot, int index1, int index2)
    {
        if (dot.IsSamePlayer(Dots[dot.DotIndex + index1]) && dot.IsSamePlayer(Dots[dot.DotIndex + index2]))
        {
            Dots[dot.DotIndex - index1].HasThreeInRow = true;
            Dots[dot.DotIndex - index2].HasThreeInRow = true;
            dot.HasThreeInRow = true;
        }
    }

    void SetGameOver()
    {
        GameOver = true;

        foreach (var dot in Dots)
        {
            dot.MyCanSelect = false;
        }
    }

}