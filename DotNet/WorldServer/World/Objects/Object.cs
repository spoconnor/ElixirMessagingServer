using System;
namespace Sean.World
{
    public class Object
    {
        public Object ()
        {
        }

        public string Name { get; set; }
        public Location Location { get; set; }
  
    public WorldTime created;
    public WorldTimeSpan maxAge
    public bool canPass;
    public int size;
    public int weight;
    public int burnEnergy;
    }
}

public struct WorldTime
{
  int Date;
  int Time;
}
public struct WorldTimeSpan;
{
  int Days;
  int Hours;
}

enum Alignment {
 Evil,
 Neutral,
 Good
}

public class CreatureStats : Object {
  int typeId;
  string name;
  WorldTimeSpan maxAge
  Alignment alignment;
  int maxSize;
  int maxWeight;
  int maxHealth;
  int maxIntelligence;
  int maxWisdom;
  int maxDexterity;
  int maxMagic;
  int maxStrength;
  int speed;
  bool canMove;
}

Dictionary<int, CreatureStats> creatureStats;

public class Creature : Object {
  int typeId;
  int health;
  int intelligence;
  int wisdom;
  int dexterity;
  int magic;
  int strength;
  int speed;
}

public abstract class Animal : IsAlive {
  override const bool canMove = true;
}

public abstract class Weapon : Object {
  int damage;
  int maxDamage;
  int minDamage;
  int wear;
  bool canRepair;
  abstract static int range;
}
public class Sword : Weapon {
  int damage = 10;
  override static int range = 1;
  override static canRepair = true;
}
public class Food : Object {
  abstract static int nutrition;
  abstract static WorldTimeSpan expiry;
}
public class Bread : Food {
  override static int nutrition = 10;
  override static  WorldTimeSpan expiry;
}
public Wood : Object {
  override static public int burnEnergy = 100;
}




