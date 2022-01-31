using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Symmetric tensor with odd size given by sz param, row-major storage and indexed by numbers from range [-sz/2; sz/2].
/// </summary>
class Tensor<T>
{
    private T[] _storage;
    private readonly T _defaultValue;
    private int _indexOffset;

    public int Sz { get; private set; }
    public int Dims { get; }
    public IReadOnlyList<T> Storage => _storage;

    public Tensor(int sz = 3, int dims = 3, T defaultValue = default)
    {
        Debug.Assert(sz % 2 != 0);

        Sz = sz;
        Dims = dims;
        _defaultValue = defaultValue;
        _storage = new T[(int) Math.Pow(sz, Dims)];
        _indexOffset = sz / 2;
        TryFillWithDefault(_storage);
    }


    private void TryFillWithDefault(T[] arr)
    {
        if (!Equals(_defaultValue, default(T)))
        {
            Array.Fill(arr, _defaultValue);
        }
    }

    public bool InRange(params int[] coords)
    {
        return coords.All(i => i + _indexOffset >= 0) && CalcInd(coords) < _storage.Length;
    }

    private int CalcInd(params int[] coords)
    {
        //#3 DIM: zi * (W * H) + yi * W + xi;
        return coords
            .Select(i => i + _indexOffset)
            .Select((val, ind) => val * (int)Math.Pow(Sz, ind))
            .Sum();
    }

    public ref T At(params int[] coords)
    {
        var ind = CalcInd(coords);
        return ref _storage[ind];
    }

    public void Expand()
    {
        var newSz = Sz + 2;
        var newStorage = new T[(int)Math.Pow(newSz,Dims)];
        TryFillWithDefault(newStorage);

        var cart = Enumerable.Range(0, Dims - 1)
            .Select(i => Enumerable.Range(1, Sz)).ToArray();


        if (Dims == 4)
        {
            foreach (var i in cart[0])
            {
                foreach (var j in cart[1])
                {
                    foreach (var k in cart[2])
                    {
                        Array.Copy(_storage,   (k-1)*Sz + (j - 1) * Sz * Sz + Sz*Sz*Sz*(i-1), newStorage, k*newSz + j*newSz*newSz + newSz*newSz*newSz*i + 1, Sz);
                    }
                }
            }
        }

        if (Dims == 3)
        {
            foreach (var i in cart[0])
            {
                foreach (var j in cart[1])
                {
                    Array.Copy(_storage, (j - 1) * Sz + Sz*Sz*(i-1), newStorage, j*newSz + newSz*newSz*i + 1, Sz);
                }
            }
        }


        // for (int i = 1; i <= Z; i++)
        // {
        //     for (int j = 1; j <= H; j++)
        //     {
        //         Array.Copy(_storage, (j - 1) * W + W*H*(i-1), newStorage, j*newW + newW*newH*i + 1, W);
        //     }
        // }
        

        _storage = newStorage;
        Sz = newSz;
        _indexOffset = Sz / 2;
    }


    public void ExpandIfOnEdge(params int[] coords)
    {
        if (coords.Any(i => Math.Abs(i) == Sz / 2))
        {
            Expand();
        }
    }
}