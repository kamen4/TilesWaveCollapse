using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesWaveCollapse.Generator;

public class Tile
{
    public int img;
    public readonly int[,] sidesTypes;
    readonly int typesBySide;
    public int Rotation { get; private set; }
    public Tile(int _typesBySide, int[] _types)
    {
        Rotation = 0;
        typesBySide = _typesBySide;
        if (_types.Length != 4 * _typesBySide)
            throw new ArgumentException("Argument length doesnt fit _typesBySide", nameof(_typesBySide));
        sidesTypes = new int[4, _typesBySide];
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < _typesBySide; j++)
                sidesTypes[i, j] = _types[i * _typesBySide + j];
    }
    Tile (int _typesBySide)
    {
        Rotation = 0;
        typesBySide = _typesBySide;
        sidesTypes = new int[4, _typesBySide];
    }
    public List<Tile> GetAllRotations()
    {
        List<Tile> answ = new();
        answ.Add(this);
        for (int i = 0; i < 3; i++)
        {
            Tile temp = new(typesBySide) { Rotation = i + 1, img = this.img};
            for (int j = 0; j < 4; j++)
                for (int k = 0; k < typesBySide; k++)
                    temp.sidesTypes[j, k] = sidesTypes[(j + i + 1) % 4, k];
            if (answ.Exists(x => x == temp))
                continue;
            answ.Add(temp);
        }
        return answ;
    }
    public bool CanConnectTo(Tile _other, Side _thisSide)
    {
       /* for (int i = 0; i < typesBySide; i++)
            if (sidesTypes[(int)_thisSide, i] != _other.sidesTypes[((int)_thisSide + 2) % 4, typesBySide - 1 - i])
                return false;*/
        return sidesTypes[(int)_thisSide, 0] == _other.sidesTypes[((int)_thisSide + 2) % 4, 0];
    }
    public static bool operator !=(Tile _t1, Tile _t2) => !(_t1 == _t2);
    public static bool operator ==(Tile _t1, Tile _t2)
    {
        if (_t1.typesBySide != _t2.typesBySide) return false;
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < _t1.typesBySide; j++)
                if (_t1.sidesTypes[i, j] != _t2.sidesTypes[i, j])
                    return false;
        return true;
    }
}
