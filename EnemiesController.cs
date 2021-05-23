﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Game
{
    public class EnemiesController
    {
        public List<EnemyController> Controllers { get; }

        public EnemiesController(IEnumerable<Enemy> enemies, Player player, Level level)
        {
            Controllers = new List<EnemyController>();
            
            foreach (var e in enemies)
                Controllers.Add(new EnemyController(e, player, level));
        }

        public void OnTick(object s, EventArgs eventArgs)
        {
            foreach (var e in Controllers)
            {
                e.Move().Start();
            }
        }
    }
}