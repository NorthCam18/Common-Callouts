using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using CalloutAPI;

namespace DomesticDispute
{
    [CalloutProperties("DomesticDispute", "NorthCam18", "1.0", Probability.Medium)]
    public class DomesticDispute : CalloutAPI.Callout
    {
        Ped suspect, victim;
        private bool dialogueFinished = false;
        private PedHash[] maleHashList = new PedHash[] { PedHash.Bodybuild01AFM, PedHash.Business01AMM, PedHash.Cyclist01, PedHash.Epsilon01AMY };
        private PedHash[] femaleHashList = new PedHash[] { PedHash.Bevhills01AFY, PedHash.Business02AFY, PedHash.Golfer01AFY, PedHash.Hipster01AFY };

        public DomesticDispute()
        {
            Random random = new Random();
            float offsetX = random.Next(100, 600);
            float offsetY = random.Next(100, 600);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));

            this.ShortName = "Domestic Dispute";
            this.CalloutDescription = "Neighbours have reported loud screaming and fighting at the residence! Things are heating up, get there quick!";
            this.ResponseCode = 3;
            this.StartDistance = 30f;
        }

        public async override Task Init()
        {
            OnAccept();
            
            Random random = new Random();

            var selectedMale = maleHashList[random.Next(maleHashList.Length)];
            var selectedFemale = femaleHashList[random.Next(femaleHashList.Length)];

            suspect = await SpawnPed(selectedMale, Location);
            victim = await SpawnPed(selectedFemale, Location);

            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
            victim.AlwaysKeepTask = true;
            victim.BlockPermanentEvents = true;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);

            Conversate();
            if(dialogueFinished == true)
            {
                suspect.Task.FightAgainst(victim);
                victim.Task.ReactAndFlee(suspect);
            }
        }

        private void Conversate()
        {
            DrawSubtitle("~r~[SUSPECT]~w~ Well you never cleaned up the house when I told you to!", 5000);
            Wait(5000);
            DrawSubtitle("~b~[VICTIM]~w~ Well you never let me go out with my friends!", 5000);
            Wait(5000);
            DrawSubtitle("~r~[SUSPECT]~w~ Oh my GOD! I'm tired of your complaining!", 5000);
            Wait(5000);
            dialogueFinished = true;
        }

        private void Notify(string message)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(message);
            DrawNotification(false, false);
        }

        private void DrawSubtitle(string message, int duration)
        {
            BeginTextCommandPrint("STRING");
            AddTextComponentSubstringPlayerName(message);
            EndTextCommandPrint(duration, false);
        }
    }
}
