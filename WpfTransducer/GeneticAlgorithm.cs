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
    class GeneticAlgorithm : SolverForDoubles
    {
        public List<double> solution
        {
            get
            {
                int nextUnreadyIndex = findNextUnready(lastIndexDelivered);
                if(nextUnreadyIndex>=0)
                {
                    lastIndexDelivered=nextUnreadyIndex;
                    return individuals[lastIndexDelivered].listOfDoubles;
                }
                else
                {
                    runGeneration();
                    lastIndexDelivered = findNextUnready(-1);
                    //The next line causes every individual to be reevaluated every time
                    resetAllFitnessScores();
                    GC.Collect();
                    return individuals[lastIndexDelivered].listOfDoubles;                    
                }
            }
        }
        public void resetAllFitnessScores()
        {
            for(int i=0;i<individuals.Count;i++)
            {
                individuals[i].score = double.NegativeInfinity;
            }
            bestIndividual.score = double.NegativeInfinity;
        }



        private int findNextUnready(int lastIndexDelivered)
        {
            for (int i = 1; i <= individuals.Count; i++)
            {
                if (individuals[(lastIndexDelivered + i) % individuals.Count].score == Double.NegativeInfinity)
                {
                    return (lastIndexDelivered + i) % individuals.Count;
                }
            }
            return -1;
        }

        private void shuffle()
        {
            for (int i = individuals.Count - 1; i > 0; i--)
            {
                int swapI = G.random.Next(i+1);
                if (swapI != i)
                {
                    swapInList(i, swapI);
                }
            }
        }

        private void swapInList(int i, int swapI)
        {
            Individual temp = individuals[i];
            individuals[i] = individuals[swapI];
            individuals[swapI] = temp;
        }

        private void sortInList(int start,int count)
        {
            for (int i = start; i < start+count-1; i++)
            {
                int bigI=i;
                double bigD = individuals[bigI].score;
                for (int j = i + 1; j < start + count; j++)
                {
                    if (individuals[j].score > bigD)
                    {
                        bigI = j;
                        bigD = individuals[bigI].score;
                    }
                }
                if (bigI != i) swapInList(i, bigI);
            }
        }



        private void runGeneration()
        {
            Mating();
            Mutation();
        }

        double probabilityOfIndividualMutation = 0.1;
        double probabilityOfDoubleMutation = 0.05;
        double rangeOfDoubleMutation = 0.2;

        private void Mutation()
        {
            for (int i = 0; i < individuals.Count; i++)
            {
                if (G.randDouble() < probabilityOfIndividualMutation)
                {
                    individuals[i].mutate(probabilityOfDoubleMutation, rangeOfDoubleMutation);
                }
            }
        }

        private void Mating()
        {
            shuffle();
            for (int i = 0; i < individuals.Count; i += 4)
            {
                //sort 4 by score
                sortInList(i, 4);
                Mate(i, i + 1, i + 2, i + 3);
                bestIndex = -1;
            }
        }

        private void Mate(int parent1I,int parent2I,int child1I,int child2I)
        {
            int size = individuals[child1I].size;
            int start = 0;
            int end = size-1;
            while ((start == 0) && (end == size - 1))
            {
                start = G.random.Next(size);
                end = G.random.Next(size);
                if (start > end)
                {
                    int temp = start;
                    start = end;
                    end = temp;
                }
            }
            individuals[child1I].copy(individuals[parent1I]);
            individuals[child1I].getCrossoverFrom(individuals[parent2I], start, end);
            individuals[child2I].copy(individuals[parent2I]);
            individuals[child2I].getCrossoverFrom(individuals[parent1I], start, end);
        }





        public List<double> bestSolutionSoFar
        {
            get
            {
                return bestIndividual.listOfDoubles;
            }
        }
        public double bestScoreSoFar
        {
            get
            {
                return bestIndividual.score;
            }
        }

        public double scoreOfLastSolution
        {
            set
            {
                individuals[lastIndexDelivered].score = value;
                if (value > bestScoreSoFar)
                {
                    bestIndex = lastIndexDelivered;
                    bestIndividual.copy(individuals[bestIndex]);
                }
            }
        }

        private Individual bestIndividual = new Individual();
        private int bestIndex = 0;
        private int lastIndexDelivered = -1;
        private List<Individual> individuals = new List<Individual>();
        public void populate(int popSize,int indSize,double min, double max)
        {
            individuals.Clear();
            for(int i=0;i<popSize;i++)
            {
                Individual ind= new Individual();
                ind.populate(indSize,min,max);
                individuals.Add(ind);
            }
            bestIndividual.populate(indSize, min,max);
            bestIndividual.copy(individuals[0]);
            bestIndex = 0;
        }

        public int size
        {
            get
            {
                return individuals.Count;
            }
        }

        internal List<double> listOfDoubles(int i)
        {
            return individuals[i].listOfDoubles;
        }
    }
}
