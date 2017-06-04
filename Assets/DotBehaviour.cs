using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * ToDo list
 * 
 * [ ] Put 9 dots each on the screen
 * [x] Move dots around to free spots on the line
 * [ ] Remove oponent dot on three in a row
 * [x] Jump when three dots are left
 */

[Serializable]
public class DotBehaviour : MonoBehaviour
{
    public string Jau { get; set; }

    public double CurrentScaleSin;
    public double Scaler = 4.0;
    public double Speed = 8;
    public bool MyCanSelect;
    public Mode CurrentMode;
    private Vector3 initialLocalScale;
    private Vector3 notUsedLocalScale;
    private Vector3 inUseLocalScale;
    public float CannotSelectAnimation;
    private double CannotSelectSin;
    private static MyContext Context = MyContext.Current;
    public bool isWhite;
    private bool NeedUpdate;
    internal int DotIndex;
    public List<int> canTouch = new List<int>();
    internal bool HasThreeInRow;


    internal bool IsCurrentColor
    {
        get
        {
            return CurrentMode == Mode.IsPlayer && isWhite == Context.IsCurrentPlayerWhite;
        }
    }

    internal bool MyIsSelected
    {
        get
        {
            return Context.CurrentlySelectedDot == this;
        }
    }

    /*internal bool HasThreeInARow
    {
        get
        {
            if(CurrentMode == Mode.IsPlayer)
            {
                if((DotIndex & 1))
                foreach (var i in CurrentlySelectedDot.canTouch)
                {
                    Dots[i];
                }
            }
        }
    }*/

    internal void ResetDot()
    {
        CurrentMode = Mode.NotUsed;
        MyCanSelect = true;
        HasThreeInRow = false;
        isWhite = false;
        CurrentScaleSin = 0.0F;
    }

    // Use this for initialization
    void Start()
    {
        char[] aaa = { '(', ')', ' ' };
        DotIndex = Convert.ToInt32(this.name.Substring(3).Trim(aaa), 10);
        Context.RegisterDot(DotIndex, this);
        initialLocalScale = transform.localScale;
        notUsedLocalScale = initialLocalScale;
        inUseLocalScale = new Vector3(initialLocalScale.x * 1.5F, initialLocalScale.y * 4.5F, initialLocalScale.z * 1.5F);

        var CircleIndex = DotIndex % 8;
        var CircleNumber = DotIndex / 8;

        if ((DotIndex & 1) == 0)
        {
            // Corner
            if (CircleIndex == 0)
            {
                canTouch.Add(DotIndex + 7);
                canTouch.Add(DotIndex + 1);
            }
            else if (CircleIndex == 7)
            {
                canTouch.Add(DotIndex - 7);
                canTouch.Add(DotIndex - 1);
            }
            else
            {
                canTouch.Add(DotIndex + 1);
                canTouch.Add(DotIndex - 1);
            }
        }
        else
        {
            // Middle
            if (CircleIndex == 7)
            {
                canTouch.Add(DotIndex - 7);
                canTouch.Add(DotIndex - 1);
            }
            else
            {
                canTouch.Add(DotIndex - 1);
                canTouch.Add(DotIndex + 1);
            }


            if (CircleNumber == 0)
            {
                canTouch.Add(DotIndex + 8);
            }
            else if (CircleNumber == 1)
            {
                canTouch.Add(DotIndex + 8);
                canTouch.Add(DotIndex - 8);
            }
            else
            {
                canTouch.Add(DotIndex - 8);
            }

        }

        ResetDot();
    }

    internal bool IsSamePlayer(DotBehaviour other)
    {
        return CurrentMode == Mode.IsPlayer && CurrentMode == other.CurrentMode && isWhite == other.isWhite;
    }

    void OnMouseDown()
    {
        if (Context.GameOver)
        {
            CannotSelectAnimation = 0.8F;
            CannotSelectSin = 0.0F;
            return;
        }

        try
        {
            if (MyCanSelect)
            {
                var DoSwitchPlayer = false;

                if (CurrentMode == Mode.NotUsed)
                {
                    if (Context.DotsLeftToPlay == 0)
                    {
                        CurrentMode = Mode.IsPlayer;
                        isWhite = Context.IsCurrentPlayerWhite;

                        Context.CurrentlySelectedDot.CurrentMode = Mode.NotUsed;

                        DoSwitchPlayer = true;
                    }
                    else
                    {
                        CurrentMode = Mode.IsPlayer;
                        isWhite = Context.IsCurrentPlayerWhite;

                        MyCanSelect = false;

                        if (--Context.DotsLeftToPlay == 0)
                        {
                            foreach (var dot in Context.Dots)
                            {
                                dot.MyCanSelect = false;
                            }
                        }

                        if (isWhite)
                        {
                            Context.DotsLeftWhite++;
                        }
                        else
                        {
                            Context.DotsLeftBlack++;
                        }

                        DoSwitchPlayer = true;
                    }
                }
                else if (CurrentMode == Mode.IsPlayer)
                {
                    if (IsCurrentColor)
                    {
                        Context.CurrentlySelectedDot = MyIsSelected ? null : this;
                    }
                    else
                    {
                        if (isWhite)
                        {
                            Context.DotsLeftWhite--;
                        }
                        else
                        {
                            Context.DotsLeftBlack--;
                        }

                        CurrentMode = Mode.NotUsed;
                        MyCanSelect = false;

                        DoSwitchPlayer = true;
                    }
                }

                Context.UpdateDots(this, DoSwitchPlayer);
            }
            else
            {
                CannotSelectAnimation = 0.8F;
                CannotSelectSin = 0.0F;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 baseSize = initialLocalScale;

        if (CurrentMode == Mode.NotUsed)
        {
            baseSize = notUsedLocalScale;
            GetComponent<Renderer>().material.color = new Color(1.0F, 0.0F, 0.0F); //C#
        }
        else if (CurrentMode == Mode.IsPlayer)
        {
            baseSize = inUseLocalScale;
            if (this.isWhite)
            {
                GetComponent<Renderer>().material.color = new Color(1.0F, 1.0F, 1.0F); //C#
            }
            else
            {
                GetComponent<Renderer>().material.color = new Color(0.0F, 0.0F, 0.0F); //C#
            }
        }

        if (MyIsSelected)
        {
            CurrentScaleSin += Speed * 4.0F * Time.deltaTime;

            var ScaleValue = ((Math.Sin(CurrentScaleSin * Scaler * Math.PI) + 1.0) * 0.5) + 0.9;
            baseSize = new Vector3(baseSize.x * (float)ScaleValue, baseSize.y, baseSize.x * (float)ScaleValue);
        }
        else if (MyCanSelect)
        {
            CurrentScaleSin += Speed * Time.deltaTime;

            var ScaleValue = ((Math.Sin(CurrentScaleSin * Scaler * Math.PI) + 1.0) * 0.5) + 0.9;
            baseSize = new Vector3(baseSize.x * (float)ScaleValue, baseSize.y, baseSize.x * (float)ScaleValue);
        }

        if (CannotSelectAnimation > 0.0F)
        {
            CannotSelectAnimation -= Time.deltaTime;
            CannotSelectSin += Speed * 10.0F * Time.deltaTime;

            var ScaleValue = ((Math.Sin(CannotSelectSin * Scaler * Math.PI) + 1.0) * 0.5) + 0.9;
            baseSize = new Vector3(baseSize.x * (float)ScaleValue, baseSize.y, baseSize.x * (float)ScaleValue);
        }

        transform.localScale = baseSize;
    }

    public enum Mode
    {
        NotUsed,
        IsPlayer,
    }
}
