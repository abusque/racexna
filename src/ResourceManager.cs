using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RaceXNA
{
   class RessourceNotFoundException : ApplicationException { }

   public class ResourceManager<T>
   {
      private Object LockObj = new Object();
      public RacingGame RaceGame { get; protected set; }
      List<BaseResource<T>> Resources;
      const int FILE_NOT_FOUND = -1;

      public ResourceManager(RacingGame game)
      {
         RaceGame = game;
         Resources = new List<BaseResource<T>>();
      }

      public void Add(string path)
      {

         BaseResource<T> fileToAdd = new BaseResource<T>(RaceGame.Content, path);

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
         BaseResource<T> fileToFind = new BaseResource<T>(RaceGame.Content, name);
         int fileIndex = Resources.IndexOf(fileToFind);

         if (fileIndex == FILE_NOT_FOUND)
         {
            throw new RessourceNotFoundException();
         }

         return Resources[fileIndex].ResourceData;
      }
   }
}
