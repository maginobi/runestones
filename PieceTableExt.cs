using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public static class PieceTableExt
    {
        public static Vector2Int GetPieceIndexVec(this PieceTable table, string pieceName)
        {
            int cat = -1;
            return table.GetPieceIndexVec(pieceName, ref cat);
        }

        public static Vector2Int GetPieceIndexVec(this PieceTable table, string pieceName, ref int category)
        {
            var pieces = (List<List<Piece>>)typeof(PieceTable).GetField("m_availablePieces", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(table);
            for (category = (category >= 0 && category < pieces.Count) ? category : 0; category < pieces.Count; category++)
            {
                for (int i = 0; i < pieces[category].Count; i++)
                {
                    if (pieces[category][i].m_name == pieceName)
                        return new Vector2Int(i % PieceTable.m_gridWidth, i / PieceTable.m_gridWidth);
                }
            }
            category = -1;
            return new Vector2Int(-1,-1);
        }
    }
}
