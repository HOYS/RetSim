﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RetSim.Items;
using RetSim.Units.UnitStats;

namespace RetSimDesktop
{
    /// <summary>
    /// Interaction logic for GearSlotSelect.xaml
    /// </summary>
    public partial class WeaponSlotSelect : UserControl
    {
        public IEnumerable<EquippableWeapon> WeaponList
        {
            get => (IEnumerable<EquippableWeapon>)GetValue(WeaponListProperty);
            set => SetValue(WeaponListProperty, value);
        }

        public static readonly DependencyProperty WeaponListProperty = DependencyProperty.Register(
            "WeaponList", 
            typeof(IEnumerable<EquippableWeapon>),
            typeof(WeaponSlotSelect));

        public EquippableWeapon SelectedItem
        {
            get => (EquippableWeapon)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(EquippableWeapon),
            typeof(WeaponSlotSelect));

        public WeaponSlotSelect()
        {
            InitializeComponent();

            gearSlot.SetBinding(DataGrid.ItemsSourceProperty, new Binding("WeaponList")
            {
                Source = this,
                Mode = BindingMode.OneWay
            });

            gearSlot.SetBinding(DataGrid.SelectedItemProperty, new Binding("SelectedItem")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });

            StrColumn.Binding = new Binding("Stats[" + StatName.Strength + "]");
            APColumn.Binding = new Binding("Stats[" + StatName.AttackPower + "]");
            AgiColumn.Binding = new Binding("Stats[" + StatName.Agility + "]");
            CritColumn.Binding = new Binding("Stats[" + StatName.CritRating + "]");
            HitColumn.Binding = new Binding("Stats[" + StatName.HitRating + "]");
            HasteColumn.Binding = new Binding("Stats[" + StatName.HasteRating + "]");
            ExpColumn.Binding = new Binding("Stats[" + StatName.ExpertiseRating + "]");
            APenColumn.Binding = new Binding("Stats[" + StatName.ArmorPenetration + "]");
            StaColumn.Binding = new Binding("Stats[" + StatName.Stamina + "]");
            IntColumn.Binding = new Binding("Stats[" + StatName.Intellect + "]");
            MP5Column.Binding = new Binding("Stats[" + StatName.ManaPer5 + "]");
            SPColumn.Binding = new Binding("Stats[" + StatName.SpellPower + "]");
            SCritColumn.Binding = new Binding("Stats[" + StatName.SpellCritRating + "]");
            SHitColumn.Binding = new Binding("Stats[" + StatName.SpellHitRating + "]");
            SHasteColumn.Binding = new Binding("Stats[" + StatName.SpellHasteRating + "]");
        }
    }
}
