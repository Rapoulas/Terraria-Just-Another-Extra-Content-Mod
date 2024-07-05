using System;
using System.Collections.Generic;
using RappleMod.Content.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Globals
{
    public class MyGlobalItems : GlobalItem
    {

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.VampireKnives){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("melee damage")) line.Text = $"{item.damage} x 4-8 melee damage";
                }
            }
            if (item.type == ItemID.OnyxBlaster){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 4 ranged damage";
                }
            }
            if (item.type == ItemID.BloodRainBow){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 1-3 ranged damage";
                }
            }
            if (item.type == ItemID.DaedalusStormbow){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 3-4 ranged damage";
                }
            }
            if (item.type == ItemID.ChlorophyteShotbow){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 2-3 ranged damage";
                }
            }
            if (item.type == ItemID.FairyQueenRangedItem){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 4 ranged damage";
                }
            }
            if (item.type == ItemID.Tsunami){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 5 ranged damage";
                    if (line.Text.Contains("Shoots 5 arrows at a time")) line.Hide();
                }
            }
            if (item.type == ItemID.Phantasm){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 4 ranged damage";
                }
            }
            if (item.type == ItemID.Boomstick){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 3-4 ranged damage";
                }
            }
            if (item.type == ItemID.QuadBarrelShotgun){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 8 ranged damage";
                }
            }
            if (item.type == ItemID.Shotgun){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 3-5 ranged damage";
                }
            }
            if (item.type == ItemID.TacticalShotgun){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 6 ranged damage";
                }
            }
            if (item.type == ItemID.Xenopopper){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 4-5 ranged damage";
                }
            }
            if (item.type == ModContent.ItemType<Deliverance>()){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 6 ranged damage";
                }
            }
            if (item.type == ModContent.ItemType<Linoge>()){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("ranged damage")) line.Text = $"{item.damage} x 3 ranged damage";
                }
            }
            if (item.type == ItemID.SkyFracture){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 3 magic damage";
                }
            }
            if (item.type == ItemID.PoisonStaff){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 3-4 magic damage";
                }
            }
            if (item.type == ItemID.VenomStaff){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 4-7 magic damage";
                }
            }
            if (item.type == ItemID.BatScepter){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 2-3 magic damage";
                }
            }
            if (item.type == ItemID.Razorpine){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 2-4 magic damage";
                }
            }
            if (item.type == ItemID.BeeGun){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 1-5 magic damage";
                }
            }
            if (item.type == ItemID.BubbleGun){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 3 magic damage";
                }
            }
            if (item.type == ItemID.WaspGun){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 2-4 magic damage";
                }
            }
            if (item.type == ItemID.LaserMachinegun){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 2 magic damage";
                }
            }
            if (item.type == ItemID.LunarFlareBook){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 3 magic damage";
                }
            }
            if (item.type == ItemID.SharpTears){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 3 magic damage";
                }
            }
            if (item.type == ItemID.ShadowFlameHexDoll){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 3 magic damage";
                }
            }
            if (item.type == ItemID.FairyQueenMagicItem){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 4 magic damage";
                }
            }
            if (item.type == ItemID.SparkleGuitar){
                foreach (TooltipLine line in tooltips ){
                    if (line.Text.Contains("magic damage")) line.Text = $"{item.damage} x 2 magic damage";
                }
            }
        }
    }
}