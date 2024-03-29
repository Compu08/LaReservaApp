﻿using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TurnosFutbol.Infrastructure;
using TurnosFutbol.Models;
//using Microsoft.Practices.ObjectBuilder2;
using Xamarin.Forms.Internals;

namespace TurnosFutbol.ViewModels
{
    public class SelectCategoryViewModel
    {
        public Category Category { get; set; }
        public bool Selected { get; set; }
    }

    public class MainPageViewModel : BindableBase
    {
        public ObservableCollection<Grouping<SelectCategoryViewModel, Item>> Categories { get; set; }

        public DelegateCommand<Grouping<SelectCategoryViewModel, Item>> HeaderSelectedCommand
        {
            get
            {
                return new DelegateCommand<Grouping<SelectCategoryViewModel, Item>>(g =>
                {
                    if (g == null)
                    {
                        return;
                    }

                    g.Key.Selected = !g.Key.Selected;
                    if (g.Key.Selected)
                    {
                        Data.DataFactory.DataItems.Where(i => (i.Category.CategoryId == g.Key.Category.CategoryId))
                            .ForEach(g.Add);
                    }
                    else
                    {
                        g.Clear();
                    }
                });
            }
        }

        public MainPageViewModel()
        {
            Categories = new ObservableCollection<Grouping<SelectCategoryViewModel, Item>>();
            List<SelectCategoryViewModel> selectCategories =
                Data.DataFactory.DataItems.Select(x => new SelectCategoryViewModel { Category = x.Category, Selected = false })
                    .GroupBy(sc => new { sc.Category.CategoryId })
                    .Select(g => g.First())
                    .ToList();
            selectCategories.ForEach(sc => Categories.Add(new Grouping<SelectCategoryViewModel, Item>(sc, new List<Item>())));
        }
    }
}
