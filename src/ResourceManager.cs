using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RaceXNA
{
   class RessourceNotFoundException : ApplicationException { }

   public class ResourceManager<T>
   {
      private Object LockObj = new Object();
      public RacingGame Game { get; protected set; }
      List<BaseResource<T>> Resources;
      const int FILE_NOT_FOUND = -1;

      public ResourceManager(RacingGame game)
      {
         Game = game;
         Resources = new List<BaseResource<T>>();
      }

      public void Add(string name)
      {

         BaseResource<T> fileToAdd = new BaseResource<T>(Game.Content, name);

         if (!Resources.Contains(fileToAdd))
         {
            fileToAdd.Load();

            lock (LockObj)
            {
               Resources.Add(fileToAdd);
            }
         }
      }

      public T Find(string name)
      {
         BaseResource<T> fileToFind = new BaseResource<T>(Game.Content, name);
         int fileIndex = Resources.IndexOf(fileToFind);

         if (fileIndex == FILE_NOT_FOUND)
         {
            throw new RessourceNotFoundException();
         }

         return Resources[fileIndex].ResourceData;
      }
   }
}
