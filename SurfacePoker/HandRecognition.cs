using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Komplette Pokerlogik
    /// Poker Algorithmen © FileFoxes Media
    /// 
    /// Hand + Table erkennen (potentielle Gewinner bestimmen)
    /// Hand + Table berechnen (exakten Gewinner bestimmen)
    /// </summary>
    public class HandRecognition
    {
        Card[] cards;



        public HandRecognition()
        {
            cards = new Card[7];
        }

        public void setCard(int i, Card c)
        {
            cards[i] = c;
        }

        //########################################################### Hand Value ###########################################################
        public int getHandValueRecoPair()
        {
            int summe = 0;
            List<int> recoCards = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                recoCards.Add(cards[i].getPip().getPipValue());
            }

            recoCards.Sort();

            int index = 0;

            while (index < recoCards.Count - 1)
            {
                if (recoCards[index] == recoCards[index + 1])
                {
                    summe += recoCards[index] * 2;
                    int removevalue = recoCards[index];
                    //Remove duplicates (max 2 cards)
                    recoCards.Remove(removevalue);
                    recoCards.Remove(removevalue);
                    break;
                }
                else
                    index++;
            }

            recoCards.Sort();
            recoCards.Reverse();

            summe += recoCards[0] + recoCards[1] + recoCards[2];


            //Console.WriteLine("aaaa:   "+recoCards[0]);
            //Console.WriteLine("bbbb:   " + recoCards[1]);
            //Console.WriteLine("ccc:   " + recoCards[2]);


            return summe;

        }

        public int getHandValueRecoTwoPair()
        {
            int summe = 0;
            List<int> recoCards = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                recoCards.Add(cards[i].getPip().getPipValue());
            }

            recoCards.Sort();

            int index = 0;

            while (index < recoCards.Count - 1)
            {
                if (recoCards[index] == recoCards[index + 1])
                {
                    summe += recoCards[index] * 2;
                    int removevalue = recoCards[index];
                    //Remove duplicates (max 2 cards)
                    recoCards.Remove(removevalue);
                    recoCards.Remove(removevalue);
                }
                else
                    index++;
            }

            recoCards.Sort();
            recoCards.Reverse();

            summe += recoCards[0];


            //Console.WriteLine("twopair aaaa:   " + recoCards[0]);



            return summe;
        }

        public int getHandValueRecoThreeOfAKind()
        {
            int summe = 0;
            List<int> recoCards = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                recoCards.Add(cards[i].getPip().getPipValue());
            }

            recoCards.Sort();

            int index = 0;

            while (index < recoCards.Count - 1)
            {
                if (recoCards[index] == recoCards[index + 1] && recoCards[index] == recoCards[index + 2])
                {
                    summe += recoCards[index] * 3;
                    int removevalue = recoCards[index];
                    int removevalue1 = recoCards[index + 1];
                    int removevalue2 = recoCards[index + 2];
                    //Remove duplicates (max 2 cards)
                    recoCards.Remove(removevalue);
                    recoCards.Remove(removevalue1);
                    recoCards.Remove(removevalue2);


                }
                else
                    index++;
            }

            recoCards.Sort();
            recoCards.Reverse();

            summe += recoCards[0] + recoCards[1];


            //Console.WriteLine("threeofKind aaaa:   " + recoCards[0] + "     "+recoCards[1]+" " +summe);



            return summe;
        }

        public int getHandValueRecoFourOfAKind()
        {
            int summe = 0;
            List<int> recoCards = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                recoCards.Add(cards[i].getPip().getPipValue());
            }

            recoCards.Sort();

            int indexx = 0;
            int index = 0;
            int count = 0;

            while (indexx < recoCards.Count - 1)
            {
                if (recoCards[index] == recoCards[index + 1] && recoCards[index] == recoCards[index + 2] && recoCards[index] == recoCards[index + 3])
                {
                    summe += recoCards[index] * 4;
                    int removevalue = recoCards[index];
                    //Remove duplicates (max 2 cards)
                    recoCards.Remove(removevalue);
                    recoCards.Remove(removevalue);
                    recoCards.Remove(removevalue);
                    recoCards.Remove(removevalue);

                }
                else
                {
                    recoCards.Reverse();
                    count += 1;
                    if (count == 2)
                    {
                        index += 1;
                    }
                }
                indexx++;
            }

            //recoCards.Sort();
            //recoCards.Reverse();

            summe += recoCards[0];


            //Console.WriteLine("FourOfAKind aaaa:   " + recoCards[0]);



            return summe;
        }

        public int getHandValueRecoFlush()
        {
            int flushvalue = 0;
            int maxsuit = 0;

            int countClubs = 0;
            int countDiamonds = 0;
            int countHearts = 0;
            int countSpades = 0;

            List<int> recoCards = new List<int>();
            List<int> recoCards2 = new List<int>();

            //Karten der recoHand in einer Liste speichern Suit values
            for (int i = 0; i < cards.Length; i++)
            {
                recoCards.Add(cards[i].getSuit().getSuitValue());
            }

            //Sortieren erhöhe die Geschwindigkeit der Suche der duplikate
            recoCards.Sort();

            //max count suit value ermitteln um den flush suit zu finden
            int index = 0;
            while (index < recoCards.Count - 1)
            {

                if (recoCards[index] == 1)
                    countClubs++;
                if (recoCards[index] == 2)
                    countDiamonds++;
                if (recoCards[index] == 3)
                    countHearts++;
                if (recoCards[index] == 4)
                    countSpades++;

                index++;
            }

            if (countClubs > 4)
                maxsuit = 1;
            if (countDiamonds > 4)
                maxsuit = 2;
            if (countHearts > 4)
                maxsuit = 3;
            if (countSpades > 4)
                maxsuit = 4;


            //flush pip value des spielers ermitteln
            index = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].getSuit().getSuitValue() == maxsuit)
                    recoCards2.Add(cards[i].getPip().getPipValue());
                //flushvalue += cards[i].getPip().getPipValue();
            }

            recoCards2.Reverse();
            flushvalue += (recoCards2[0] + recoCards2[1] + recoCards2[2] + recoCards2[3] + recoCards2[4]);
            //Console.WriteLine("          flushvalue:   " + flushvalue);

            /*
            Console.WriteLine("Clubs: " + countClubs);
            Console.WriteLine("Diamonds: " + countDiamonds);
            Console.WriteLine("Hearts: " + countHearts);
            Console.WriteLine("Spades: " + countSpades);

            

            */

            //flush value max zurückgeben
            return flushvalue;

        }

        public int getHandValueRecoStraightFlush(String flush)
        {
            List<int> recoCards = new List<int>();
            int sum = 0;
            //Set values in array
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].getSuit().ToString().Equals(flush))
                    recoCards.Add(cards[i].getPip().getValue());

            }

            recoCards.Sort();

            int index = 0;

            //Duplikate entfernen
            while (index < recoCards.Count - 1)
            {
                if (recoCards[index] == recoCards[index + 1])
                    recoCards.RemoveAt(index);
                else
                    index++;
            }

            bool r1 = false;
            bool r2 = false;
            bool r3 = false;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (j == 0)
                    {
                        if (recoCards[i] != recoCards[i + 1] - 1)
                            recoCards.RemoveAt(i);
                    }

                    if (j == 1)
                    {
                        if (!r1)
                        {
                            recoCards.Reverse();
                            r1 = true;
                        }
                        if (recoCards[i] != recoCards[i + 1] + 1)
                            recoCards.RemoveAt(i);
                    }


                    if (j == 2)
                    {
                        if (!r2)
                        {
                            recoCards.Reverse();
                            r2 = true;
                        }
                        if (recoCards[i] != recoCards[i + 1] - 1)
                            recoCards.RemoveAt(i);
                    }

                    if (j == 3)
                    {
                        if (!r3)
                        {
                            recoCards.Reverse();
                            r3 = true;
                        }
                        if (recoCards[i] != recoCards[i + 1] + 1)
                            recoCards.RemoveAt(i);
                    }





                }
            }

            index = 0;
            while (index < recoCards.Count)
            {

                sum += recoCards[index];
                index++;
                if (index >= 5)
                    break;
            }


            return sum;

        }

        public int getHandValueRecoStraight()
        {
            List<int> recoCards = new List<int>();
            int sum = 0;
            //Set values in array
            for (int i = 0; i < cards.Length; i++)
            {
                recoCards.Add(cards[i].getPip().getValue());
            }

            recoCards.Sort();

            int index = 0;

            //Duplikate entfernen
            while (index < recoCards.Count - 1)
            {
                if (recoCards[index] == recoCards[index + 1])
                    recoCards.RemoveAt(index);
                else
                    index++;
            }

            bool r1 = false;
            bool r2 = false;
            bool r3 = false;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (j == 0)
                    {
                        if (recoCards[i] != recoCards[i + 1] - 1)
                            recoCards.RemoveAt(i);
                    }

                    if (j == 1)
                    {
                        if (!r1)
                        {
                            recoCards.Reverse();
                            r1 = true;
                        }
                        if (recoCards[i] != recoCards[i + 1] + 1)
                            recoCards.RemoveAt(i);
                    }


                    if (j == 2)
                    {
                        if (!r2)
                        {
                            recoCards.Reverse();
                            r2 = true;
                        }
                        if (recoCards[i] != recoCards[i + 1] - 1)
                            recoCards.RemoveAt(i);
                    }

                    if (j == 3)
                    {
                        if (!r3)
                        {
                            recoCards.Reverse();
                            r3 = true;
                        }
                        if (recoCards[i] != recoCards[i + 1] + 1)
                            recoCards.RemoveAt(i);
                    }





                }
            }

            index = 0;
            while (index < recoCards.Count)
            {

                sum += recoCards[index];
                index++;
                if (index >= 5)
                    break;
            }


            return sum;

        }

        public int getHandValueFullHouse()
        {
            String[] values = new String[7];
            int counter = 0;
            int sum = 0;
            List<int> recoCards = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                values[i] = cards[i].getPip().ToString();
            }

            for (int x = 0; x < values.Length; x++)
            {
                for (int y = 0; y < cards.Length; y++)
                {
                    if (values[x].Equals(cards[y].getPip().ToString()))
                    {
                        //recoCards.Add(cards[x].getPip().getPipValue());
                        counter++;
                    }
                }
                if (counter > 1 && counter < 6)
                {
                    recoCards.Add(cards[x].getPip().getPipValue());
                    //sum+=cards[x].getPip().getPipValue();
                }

                counter = 0;
            }
            recoCards.Reverse();
            sum += (recoCards[0] + recoCards[1] + recoCards[2] + recoCards[3] + recoCards[4]);
            return sum;


        }
        //########################################################### Other ###########################################################
        /*public override String ToString()
        {
            String str = "";

            for (int i = 0; i < cards.Length; i++)
            {
                str += "\t" + (i + 1) + ": ";
                str += cards[i];
                str += "\n";
            }

            return str;
        }*/

        public int getHighestCard()
        {
            int hightest = 0;
            String[] values = new String[7];
            //Put each cards numeric value into array
            for (int i = 0; i < cards.Length; i++)
            {
                if (hightest < cards[i].getPip().getPipValue())
                    hightest = cards[i].getPip().getPipValue();
            }

            return hightest;

        }

        //########################################################### Check Hands true false ###########################################################
        public Boolean isFourOfAKind()
        {
            String[] values = new String[7];
            int counter = 0;

            for (int i = 0; i < cards.Length; i++)
            {
                values[i] = cards[i].getPip().ToString();
            }

            //Same process as isPair(), except return true for 4 matches
            for (int x = 0; x < values.Length; x++)
            {
                for (int y = 0; y < cards.Length; y++)
                {
                    if (values[x].Equals(cards[y].getPip().ToString()))
                        counter++;
                    if (counter == 4)
                        return true;

                }
                counter = 0;
            }

            return false;
        }

        public Boolean isFullHouse()
        {
            String[] values = new String[7];
            int counter = 0;
            int sum = 0;

            if (isFourOfAKind() == true)
                return false;

            for (int i = 0; i < cards.Length; i++)
            {
                values[i] = cards[i].getPip().ToString();
            }

            for (int x = 0; x < values.Length; x++)
            {
                for (int y = 0; y < cards.Length; y++)
                {
                    if (values[x].Equals(cards[y].getPip().ToString()))
                    {
                        counter++;
                    }
                }
                if (counter > 1)
                    sum++;

                counter = 0;
            }

            if (sum >= 5)
                return true;

            return false;
        }

        public Boolean isThreeOfAKind()
        {
            String[] values = new String[7];
            int counter = 0;

            for (int i = 0; i < cards.Length; i++)
            {
                values[i] = cards[i].getPip().ToString();
            }

            //Same process as isPair(), except return true for 3 matches
            for (int x = 0; x < values.Length; x++)
            {
                for (int y = 0; y < cards.Length; y++)
                {
                    if (values[x].Equals(cards[y].getPip().ToString()))
                        counter++;
                    if (counter == 3)
                        return true;

                }
                counter = 0;
            }

            return false;
        }

        public Boolean isStraight(int ringCheck)
        {
            List<int> recoCards = new List<int>();

            //Set values in array
            for (int i = 0; i < cards.Length; i++)
            {
                recoCards.Add(cards[i].getPip().getValue());
            }

            recoCards.Sort();

            int index = 0;

            //Evenetuelle Duplikate entfernen um das zählen zu erleichtern
            while (index < recoCards.Count - 1)
            {
                if (recoCards[index] == recoCards[index + 1])
                    recoCards.RemoveAt(index);
                else
                    index++;
            }

            recoCards.Sort();

            int counter = 0;


            if (ringCheck == 1)
            {

                //Prüfen ob As vorhanden ist bzgl Ringbildung
                bool asInside = false;
                for (int i = 0; i < recoCards.Count; i++)
                {
                    if (recoCards[i] == 14)
                        asInside = true;
                }


                //Wenn AS drin dann spezielle Prüfung bzgl der fast Ringbildung 
                if (asInside)
                {
                    //As auf 1 setzten
                    for (int i = 0; i < recoCards.Count; i++)
                    {
                        if (recoCards[i] == 14)
                        {
                            recoCards.RemoveAt(i);
                            recoCards.Add(1);
                        }

                    }
                    recoCards.Sort();

                    //Karten mit Abständen >1 entfernen damit das zählen funktioniert
                    for (int i = 0; i < recoCards.Count - 1; i++)
                    {
                        if (recoCards[i] != recoCards[i + 1] - 1)
                            if (i == 0)
                                recoCards.RemoveAt(i);
                            else
                                recoCards.RemoveAt(i + 1);

                    }

                    //Zählen ob fünf aufeinander folgende karten
                    for (int i = 0; i < recoCards.Count - 1; i++)
                    {
                        if (recoCards[i] == recoCards[i + 1] - 1)
                            counter++;
                    }

                }
                else
                {
                    return false;
                }
            }
            else if (ringCheck == 0)
            {
                //Karten mit Abständen >1 entfernen damit das zählen funktioniert
                for (int i = 0; i < recoCards.Count - 1; i++)
                {
                    if (recoCards[i] != recoCards[i + 1] - 1)
                        if (i == 0)
                            recoCards.RemoveAt(i);
                        else
                            recoCards.RemoveAt(i + 1);

                }

                //Zählen ob fünf aufeinander folgende karten
                for (int i = 0; i < recoCards.Count - 1; i++)
                {
                    if (recoCards[i] == recoCards[i + 1] - 1)
                        counter++;
                }

            }




            //wenn der zähler >= 4 dann wurden min 5 aufeinanderfolgende karten gefunden
            if (counter >= 4)
                return true;


            return false;
        }

        public Boolean isRoyalFlush()
        {
            if (isFlush() == false || isStraight(0) == false || isStraight(1) == false)
                return false;

            int[] values = new int[7];
            int pos;
            int temp;

            //Set values in array
            for (int i = 0; i < cards.Length; i++)
            {
                values[i] = cards[i].getPip().getValue();
                //If Ace
                if (values[i] == 1)
                    values[i] = 14;
            }

            //Sort Numerically
            for (int i = 1; i < values.Length; i++)
            {
                pos = i;
                while (pos != 0)
                {
                    if (values[pos] < values[pos - 1])
                    {
                        temp = values[pos];
                        values[pos] = values[pos - 1];
                        values[pos - 1] = temp;
                    }
                    pos--;
                }
            }

            //Royal flush is a straight flush, with the lowest card being a 10	
            if (values[0] == 10)
                return true;

            return false;
        }

        public Boolean isStraightFlush()
        {
            //If theres a straight and a flush present
            if ((isStraight(0) == true || isStraight(1)) && isFlush() == true)
                return true;

            return false;
        }

        public String getFlush()
        {
            int countClubs = 0;
            int countDiamonds = 0;
            int countHearts = 0;
            int countSpades = 0;

            int index = 0;

            while (index < cards.Length)
            {
                if (cards[index].getSuit().getSuitValue() == 1)
                    countClubs++;
                if (cards[index].getSuit().getSuitValue() == 2)
                    countDiamonds++;
                if (cards[index].getSuit().getSuitValue() == 3)
                    countHearts++;
                if (cards[index].getSuit().getSuitValue() == 4)
                    countSpades++;

                index++;
            }

            //Console.WriteLine("Clubs: " + countClubs);
            //Console.WriteLine("Diamonds: " + countDiamonds);
            //Console.WriteLine("Hearts: " + countHearts);
            //Console.WriteLine("Spades: " + countSpades);

            if (countClubs > 4)
                return "Kreuz";
            if (countDiamonds > 4)
                return "Karo";
            if (countHearts > 4)
                return "Herz";
            if (countSpades > 4)
                return "Pik";

            return "";

        }

        public Boolean isFlush()
        {
            int countClubs = 0;
            int countDiamonds = 0;
            int countHearts = 0;
            int countSpades = 0;

            int index = 0;

            while (index < cards.Length)
            {
                if (cards[index].getSuit().getSuitValue() == 1)
                    countClubs++;
                if (cards[index].getSuit().getSuitValue() == 2)
                    countDiamonds++;
                if (cards[index].getSuit().getSuitValue() == 3)
                    countHearts++;
                if (cards[index].getSuit().getSuitValue() == 4)
                    countSpades++;

                index++;
            }

            //Console.WriteLine("Clubs: " + countClubs);
            //Console.WriteLine("Diamonds: " + countDiamonds);
            //Console.WriteLine("Hearts: " + countHearts);
            //Console.WriteLine("Spades: " + countSpades);

            if (countClubs > 4 || countDiamonds > 4 || countHearts > 4 || countSpades > 4)
                return true;
            return false;
        }

        public Boolean isTwoPair()
        {
            String[] values = new String[7];
            int counter = 0;
            int sum = 0;

            //Two pairs can resemble 4 of a kind
            if (isFourOfAKind() == true)
                return false;

            for (int i = 0; i < cards.Length; i++)
            {
                values[i] = cards[i].getPip().ToString();
            }

            for (int x = 0; x < values.Length; x++)
            {
                for (int y = 0; y < cards.Length; y++)
                {
                    if (values[x].Equals(cards[y].getPip().ToString()))
                    {
                        counter++;
                    }
                }
                if (counter > 1)
                    sum++;

                counter = 0;
            }

            if (sum == 4)
                return true;

            return false;
        }

        public Boolean isPair()
        {
            String[] values = new String[7];
            int counter = 0;
            int valueCounter = 0;

            for (int i = 0; i < cards.Length; i++)
            {
                values[i] = cards[i].getPip().ToString();
            }



            for (int x = 0; x < values.Length; x++)
            {
                for (int y = 0; y < cards.Length; y++)
                {
                    if (values[x].Equals(cards[y].getPip().ToString()))
                    {

                        counter++;
                        if (counter == 2)
                            valueCounter = cards[y].getPip().getPipValue();
                    }
                    if (counter == 2)
                    {

                        System.Console.WriteLine("v: " + valueCounter * 2);
                        return true;
                    }

                }



                counter = 0;
            }

            return false;
        }
    }
}
