using System.Collections.Generic;

namespace Misterio.Core
{
    // El tipo de indicio que hace falta para una pista
    public enum EvidenceKind { Place, Activity, Relation }

    // Quién o qué da cada pista del caso. Cada sospechoso da una y el resto son indicios repartidos
    // por las salas. Si no caben, algún sospechoso da una segunda
    public class CluePlacement
    {
        public const int SlotsPerRoom = 3; // huecos para indicios en cada sala

        public List<int>[] BySuspect; // las pistas que da cada sospechoso
        public int[,] BySlot;         // [sala, hueco], la pista de cada indicio o -1 si el hueco queda vacío

        public CluePlacement(int suspects, int rooms)
        {
            BySuspect = new List<int>[suspects];
            for (int s = 0; s < suspects; s++)
                BySuspect[s] = new List<int>();

            BySlot = new int[rooms, SlotsPerRoom];
            for (int r = 0; r < rooms; r++)
            {
                for (int i = 0; i < SlotsPerRoom; i++)
                    BySlot[r, i] = -1;
            }
        }

        // Reparte las pistas con esa semilla, así el mismo caso las deja siempre en el mismo sitio
        // Devuelve null si no caben
        public static CluePlacement Place(CaseDefinition c, int seed)
        {
            if (c.Clues.Count < c.Suspects.Count)
                return null;

            System.Random random = new System.Random(seed);
            List<int> clues = Numbers(c.Clues.Count);
            List<int> suspects = Numbers(c.Suspects.Count);
            List<int> rooms = Numbers(c.Places.Count);
            CaseGenerator.Shuffle(clues, random);
            CaseGenerator.Shuffle(suspects, random);
            CaseGenerator.Shuffle(rooms, random);

            CluePlacement placement = new CluePlacement(c.Suspects.Count, c.Places.Count);
            int next = 0; // la siguiente pista por repartir

            // una para cada sospechoso
            foreach (int s in suspects)
            {
                placement.BySuspect[s].Add(clues[next]);
                next++;
            }

            // el resto a los huecos, pasando de una sala a otra para que no se amontonen
            for (int i = 0; i < rooms.Count * SlotsPerRoom && next < clues.Count; i++)
            {
                placement.BySlot[rooms[i % rooms.Count], i / rooms.Count] = clues[next];
                next++;
            }

            // si no caben una segunda a algún sospechoso
            foreach (int s in suspects)
            {
                if (next < clues.Count)
                {
                    placement.BySuspect[s].Add(clues[next]);
                    next++;
                }
            }

            if (next < clues.Count)
                return null;
            return placement;
        }

        // Lugar si la pista dice dónde estaba alguien, actividad si dice qué hacía y relación si junta las dos
        public static EvidenceKind KindOf(Clue clue)
        {
            SuspectClue suspectClue = clue as SuspectClue;
            if (suspectClue == null)
                return EvidenceKind.Relation;
            if (suspectClue.Category == Category.Place)
                return EvidenceKind.Place;
            return EvidenceKind.Activity;
        }

        // 0, 1, 2... hasta count - 1
        static List<int> Numbers(int count)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < count; i++)
                list.Add(i);
            return list;
        }
    }
}