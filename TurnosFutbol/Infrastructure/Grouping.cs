﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TurnosFutbol.Infrastructure
{
    public class Grouping<TK, T> : ObservableCollection<T>
    {
        public TK Key { get; private set; }

        public Grouping(TK key, IEnumerable<T> items)
        {
            Key = key;
            foreach (T item in items)
            {
                Items.Add(item);
            }
        }
    }
}
