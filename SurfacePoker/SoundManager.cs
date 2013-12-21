using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;

namespace SurfacePoker
{
    public class SoundManager
    {
        private String pfad;

        private Boolean sound = true;

        //ChipSound am Anfang wenn Karten und Chips an Spieler ausgegeben werden
        public void playSoundFallen6() {
            if (sound)
            {
                SoundPlayer sp = new SoundPlayer("Sounds/chips_fallen6.wav");
                sp.Load();
                sp.Play();
            }
        }

        //ChipSound wenn ein Spieler gewonnen hat
        public void playSoundViele1()
        {
            if (sound)
            {
                SoundPlayer sp = new SoundPlayer("Sounds/chips_viele1.wav");
                sp.Load();
                sp.Play();
            }
        }

        //ChipSound wenn ein Spieler Bet'et
        public void playSoundEinsatz2()
        {
            if (sound)
            {
                SoundPlayer sp = new SoundPlayer("Sounds/einsatz2.wav");
                sp.Load();
                sp.Play();
            }
        }

        //ChipSound wenn ein Chip auf den Tisch gezogen wird
        public void playSoundEinChip2()
        {
            if (sound)
            {
                SoundPlayer sp = new SoundPlayer("Sounds/einchip2.wav");
                sp.Load();
                sp.Play();
            }
        }

        /**
        //ChipSound wenn ein Chip rausgezogen wird ALTERNATIV
        public void playSoundEinsatz()
        {
            SoundPlayer sp = new SoundPlayer("Sounds/einsatz.wav");
            sp.Load();
            sp.Play();
        }
        */

        public Boolean Sound
        {
            get { return sound; }
            set { sound = value; }
        }


        public SoundManager() { }
    }
}
