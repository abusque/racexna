using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
   public class BoîteCollision : PrimitiveDeBase
   {
      const int NB_SOMMETS = 10;
      const int NB_TRIANGLES_PAR_STRIP = 8;
      Color Couleur { get; set; }
      VertexPositionColor[] Sommets { get; set; }
      Vector3 Min { get; set; }
      Vector3 Max { get; set; }
      ObjetDeBasePhysique ObjetPhysique { get; set; }
      BoundingBox Boîte { get; set; }


      public BoîteCollision(Atelier jeu, ObjetDeBasePhysique objetPhysique, BoundingBox boîte, Color couleur)
         : base(jeu)
      {
         Boîte = boîte;
         Min = Boîte.Min;
         Max = Boîte.Max;
         Couleur = couleur; 
         ObjetPhysique = objetPhysique;
         Monde = ObjetPhysique.GetMonde();
      }

      public override void Initialize()
      {
         Sommets = new VertexPositionColor[NB_SOMMETS];
         base.Initialize();
      }

      protected override void InitialiserSommets()
      {
         Vector3[] listeDesCoins = Boîte.GetCorners();//Je construis le cube à partir des coins
                                                      //de la BoundingBox (Boîte) du modèle
                                                      //reçu en paramètre (ObjetPhysique)
         Sommets[0] = new VertexPositionColor(listeDesCoins[3], Couleur); //A                      D-----------F      
         Sommets[1] = new VertexPositionColor(listeDesCoins[0], Couleur);//B                      /.          /.                 
         Sommets[2] = new VertexPositionColor(listeDesCoins[7], Couleur);//C                     / .         / .       
         Sommets[3] = new VertexPositionColor(listeDesCoins[4], Couleur);//D                    B---------- H  .       
         Sommets[4] = new VertexPositionColor(listeDesCoins[6], Couleur);//E                    |  .        |  .               
         Sommets[5] = new VertexPositionColor(listeDesCoins[5], Couleur);//F                    |  C........|. E                 
         Sommets[6] = new VertexPositionColor(listeDesCoins[2], Couleur);//G                    | /         | /       
         Sommets[7] = new VertexPositionColor(listeDesCoins[1], Couleur);//H                    |/          |/                             
         Sommets[8] = new VertexPositionColor(listeDesCoins[3], Couleur);//A                    A-----------G
         Sommets[9] = new VertexPositionColor(listeDesCoins[0], Couleur);//B
      }

      public override void Update(GameTime gameTime)
      {
         GérerClavier();
         if (!Monde.Equals(ObjetPhysique.GetMonde()))
         {
            Monde = ObjetPhysique.GetMonde(); //J'actualise le monde de la "boîte" visuelle
            InitialiserSommets();             //Je recré la boîte 
         }
         base.Update(gameTime);
      }

      private void GérerClavier()
      {
         if (Jeu.InputMgr.EstNouvelleTouche(Keys.B) && 
            (Jeu.InputMgr.EstEnfoncée(Keys.LeftShift) || Jeu.InputMgr.EstEnfoncée(Keys.RightShift)))
         {
            this.Visible = !this.Visible;
         }
      }

      public override void Draw(GameTime gameTime)
      {
         CullMode ancienCullMode = Jeu.GraphicsDevice.RenderState.CullMode;
         Jeu.GraphicsDevice.RenderState.CullMode = CullMode.None;
         FillMode ancienFillMode = Jeu.GraphicsDevice.RenderState.FillMode;
         Jeu.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
         Jeu.GraphicsDevice.VertexDeclaration = new VertexDeclaration(Jeu.GraphicsDevice, VertexPositionColor.VertexElements);
         InitialiserSommets();
         Jeu.Affichage3D.Effet3D.World = Monde;
         Jeu.Affichage3D.Effet3D.VertexColorEnabled = true;
         Jeu.Affichage3D.Effet3D.Begin();
         foreach (EffectPass passeEffet in Jeu.Affichage3D.Effet3D.CurrentTechnique.Passes)
         {
            passeEffet.Begin();
            Jeu.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip,
                                                                       Sommets, 0, NB_TRIANGLES_PAR_STRIP);
            passeEffet.End();
         }
         Jeu.Affichage3D.Effet3D.End();
         Jeu.Affichage3D.Effet3D.VertexColorEnabled = false;
         Jeu.GraphicsDevice.RenderState.FillMode = ancienFillMode;
         Jeu.GraphicsDevice.RenderState.CullMode = ancienCullMode;
         base.Draw(gameTime);
      }
   }
}