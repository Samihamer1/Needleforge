using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Needleforge.Data;

/// <summary>
/// An extension of <see cref="HeroController.ConfigGroup"/>, currently used only to
/// store an extra attack type named 'SpecialSlash' for the purpose of the Wanderer's recoil slash, 
/// which doesn't fit into regular categories. 
/// </summary>
public class ConfigGroupNeedleforge : HeroController.ConfigGroup
{
    /// <summary>
    /// An extra GameObject property to store Wanderer's recoil slash.
    /// Unneeded for any other purposes, and optional.
    /// </summary>
    public GameObject? SpecialSlash { get; set; }
}
