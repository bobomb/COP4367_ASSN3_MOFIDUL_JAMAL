//*********************************************************
//
// (c) Copyright 2014 Dr. Thomas Fernandez
// 
// All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfTransducer
{
    class Individual
    {
        public List<double> listOfDoubles
        {
            get
            {
                return _listOfDoubles;
            }
        }
        public int size
        {
            get { return _listOfDoubles.Count; }
        }
        public double score
        {
            get { return _score; }
            set { _score = value; }
        }
        public double getDouble(int x)
        {
            return _listOfDoubles[x];
        }
        public void setDouble(int x, double d)
        {
            _listOfDoubles[x] = d;
        }

        //public void copy(Individual sourse)
        //{
        //    _listOfDoubles.Clear();
        //    for (int i = 0; i < sourse.size; i++) _listOfDoubles.Add(sourse.getDouble(i));
        //    score = sourse.score;
        //}

        public void copy(Individual sourse)
        {
            for (int i = 0; i < sourse.size; i++) _listOfDoubles[i]=sourse._listOfDoubles[i];
            score = sourse.score;
        }

        public void populate(int count, double min, double max)
        {
            _listOfDoubles.Clear();
            for (int i = 0; i < count; i++) _listOfDoubles.Add(G.randDouble(min, max));
            score = Double.NegativeInfinity;
        }

        List<double> _listOfDoubles = new List<double>();
        double _score = Double.NegativeInfinity;

        internal void getCrossoverFrom(Individual other,int start,int end)
        {
            for (int i = start; i <= end; i++)
            {
                _listOfDoubles[i] = other._listOfDoubles[i];
            }
            score = Double.NegativeInfinity;
        }

        internal void mutate(double probabilityOfDoubleMutation, double rangeOfDoubleMutation)
        {
            for(int i=0;i<_listOfDoubles.Count;i++)
            {
                if (G.randDouble() < probabilityOfDoubleMutation)
                {
                    _listOfDoubles[i] += G.randDouble(-rangeOfDoubleMutation, rangeOfDoubleMutation);
                }
            }
            score = Double.NegativeInfinity;
        }
    }
}
