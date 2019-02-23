using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace Lockpicks
{
    public class Mod : StardewModdingAPI.Mod
    {
#if DEBUG
        private static readonly bool DEBUG = true;
#else
        private static readonly bool DEBUG = false;
#endif
        public static Mod Instance;
        public static bwdyworks.ModUtil ModUtil;

        public override void Entry(IModHelper helper)
        {
            ModUtil = new bwdyworks.ModUtil(this);
            Instance = this;
            if(ModUtil.StartConfig(DEBUG))
            {
                Config.Load();
                if (Config.ready)
                {
                    ModUtil.AddItem(new bwdyworks.BasicItemEntry(this, "lockpick", 30, -300, "Basic", Object.junkCategory, "Lockpick", "Used to bypass locked doors."));
                    ModUtil.AddMonsterLoot(new bwdyworks.MonsterLootEntry(this, "Green Slime", "lockpick", 0.1f));                 
                }
                helper.Events.Input.ButtonPressed += Input_ButtonPressed;
                ModUtil.EndConfig();
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (ModUtil.Debug)
            {
                if (e.Button == SButton.NumPad0)
                {
                    var id = ModUtil.GetModItemId("lockpick");
                    if (ModUtil.Debug && id != null) Monitor.Log("lockpick id: " + id.Value);

                    if (id != null) ModUtil.GiveItemToLocalPlayer(id.Value, 1);
                }
            }
            if (e.Button.IsActionButton())
            {
                if (Context.IsPlayerFree)
                {
                    var ao = Game1.player.ActiveObject;
                    if (ao != null && ao.DisplayName == "Lockpick")
                    {
                        var targetOut = ModUtil.GetLocalPlayerFacingTileCoordinate();
                        var keyOut = Game1.currentLocation.Name + "." + targetOut[0] + "." + targetOut[1];
                        if (ModUtil.Debug) Monitor.Log("Facing target: " + keyOut);

                        var targetIn = ModUtil.GetLocalPlayerStandingTileCoordinate();
                        var keyIn = Game1.currentLocation.Name + "." + targetIn[0] + "." + targetIn[1];
                        if (ModUtil.Debug) Monitor.Log("Standing at: " + keyIn);

                        //check out lock
                        var cle2 = Config.GetMatchingOutLock(keyOut);
                        if (cle2 != null)
                        {
                            Helper.Input.Suppress(e.Button);
                            ModUtil.AskQuestion("Use lockpick?", new[]{new Response(keyOut, "Yes"),new Response("No","No")}, QuestionCallbackOutLock);
                        }
                        //check in lock
                        cle2 = Config.GetMatchingInLock(keyIn);
                        if (cle2 != null)
                        {
                            Helper.Input.Suppress(e.Button);
                            ModUtil.AskQuestion("Use lockpick?", new[] { new Response(keyIn, "Yes"), new Response("No", "No") }, QuestionCallbackInLock);
                        }
                    }
                }
            }
        }

        public void QuestionCallbackOutLock(Farmer who, string answerKey)
        {
            if (answerKey != "No")
            {
                ConfigLockEnd cle2 = Config.GetMatchingOutLock(answerKey);
                if (new System.Random().Next(100) < 10) //ohno. the pick broke.
                {
                    Game1.playSound("axe");
                    Game1.showRedMessage("The lockpick broke!");
                    ModUtil.RemoveItemFromLocalPlayer(who.ActiveObject);
                    return;
                }
                Game1.playSound("axchop");
                who.warpFarmer(new Warp(cle2.MapX, cle2.MapY, cle2.MapName, cle2.MapX, cle2.MapY, false));
            }
        }

        public void QuestionCallbackInLock(Farmer who, string answerKey)
        {
            if (answerKey != "No")
            {
                ConfigLockEnd cle2 = Config.GetMatchingInLock(answerKey);
                Game1.playSound("axchop");
                who.warpFarmer(new Warp(cle2.MapX, cle2.MapY, cle2.MapName, cle2.MapX, cle2.MapY, false));
            }
        }
    }
}