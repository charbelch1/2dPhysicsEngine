using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using Flat.Graphics;
using Flat.Input;
using FlatPhysics;

using FlatMath = FlatPhysics.FlatMath;

namespace FlatPhysicsTester
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Screen screen;
        private Sprites sprites;
        private Shapes shapes;
        private Camera camera;

        private FlatWorld world;

        private List<FlatEntity> entityList;
        private List<FlatEntity> entityRemovalList;


        private Stopwatch watch;

        private double totalWorldStepTime = 0d;
        private int totalBodyCount=0;
        private int totalSampleCount=0;
        private Stopwatch sampleTimer = new Stopwatch();

        private string worldStepTimeString = string.Empty;
        private string bodyCountString = string.Empty;

        public Game1()
        {
          this.graphics = new GraphicsDeviceManager(this);
          this.graphics.SynchronizeWithVerticalRetrace = true;

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.IsFixedTimeStep=true;

            const double UpdatesPerSecond = 60d;
            this.TargetElapsedTime= TimeSpan.FromTicks((long)Math.Round((double)TimeSpan.TicksPerSecond/UpdatesPerSecond));
        }

        protected override void Initialize()
        {
            this.Window.Position = new Point(10, 40);

            FlatUtil.SetRelativeBackBufferSize(this.graphics, 0.85f);

            this.screen = new Screen(this, 1280, 768);
            this.sprites = new Sprites(this);
            this.shapes = new Shapes(this);
            this.camera = new Camera(this.screen);
            this.camera.Zoom = 20;

            this.camera.GetExtents(out float left, out float right, out float bottom, out float top);
            
            this.entityList = new List<FlatEntity>();
            this.entityRemovalList = new List<FlatEntity>();
            this.world = new FlatWorld();

            float padding =MathF.Abs((right - left)*0.1f);

           if(!FlatBody.CreateBoxBody(right - left - 2 * padding, 3f, 1f, true, 0.5f, 
                out FlatBody groundBody, out string errorMessage))
            {
                throw new Exception(errorMessage);
            }
            groundBody.MoveTo(new FlatVector(0, -10));
            this.world.AddBody(groundBody);
            this.entityList.Add(new FlatEntity(groundBody,Color.DarkGreen));

            if(!FlatBody.CreateBoxBody(20f, 2f,1,true, 0.5f, out FlatBody ledgeBody1, out string errorMsg1))
            {
                throw new Exception(errorMsg1);
            }
            ledgeBody1.MoveTo(new FlatVector(-10, 3));
            ledgeBody1.Rotate(-MathHelper.TwoPi / 20f);
            this.world.AddBody(ledgeBody1);
            this.entityList.Add(new FlatEntity(ledgeBody1, Color.DarkGray));

            if (!FlatBody.CreateBoxBody(15f, 2f,  1, true, 0.5f, out FlatBody ledgeBody2, out string errorMsg2))
            {
                throw new Exception(errorMsg2);
            }
            ledgeBody2.MoveTo(new FlatVector(10, 10));
            ledgeBody2.Rotate(MathHelper.TwoPi / 20f);
            this.world.AddBody(ledgeBody2);
            this.entityList.Add(new FlatEntity(ledgeBody2, Color.DarkRed));

            this.watch = new Stopwatch();
            this.sampleTimer.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            FlatKeyboard keyboard = FlatKeyboard.Instance;
            FlatMouse mouse = FlatMouse.Instance;

            keyboard.Update();
            mouse.Update();


            //add box body
            if (mouse.IsRightMouseButtonPressed())
            {
                float width = RandomHelper.RandomSingle(2f, 3f);
                float height = RandomHelper.RandomSingle(2f, 3f);

               FlatVector mouseWorldPosistion=
                    FlatConverter.ToFlatVector(mouse.GetMouseWorldPosition(this, this.screen, this.camera));

                this.entityList.Add(new FlatEntity(this.world, width, height, false, mouseWorldPosistion));

            }

            //add circle body
            if(mouse.IsLeftMouseButtonPressed())
            {
                float radius = RandomHelper.RandomSingle(1.25f, 1.5f);

                FlatVector mouseWorldPosistion =
                     FlatConverter.ToFlatVector(mouse.GetMouseWorldPosition(this, this.screen, this.camera));

                this.entityList.Add(new FlatEntity(this.world,radius, false, mouseWorldPosistion));

            }

            if(keyboard.IsKeyAvailable)
            {
                if(keyboard.IsKeyClicked(Keys.LeftControl))
                {

                    Console.WriteLine($"BodyCount: {this.bodyCountString}");
                    Console.WriteLine($"WorldStepTime: {this.worldStepTimeString}");
                    Console.WriteLine();
                }

                if(keyboard.IsKeyClicked(Keys.OemTilde))
                {
                    Console.WriteLine($"BodyCount: {this.world.BodyCount}");
                    Console.WriteLine($"StepTime: {Math.Round(this.watch.Elapsed.TotalMilliseconds,4)}");
                    Console.WriteLine();

                }
                if (keyboard.IsKeyClicked(Keys.Escape))
                {
                    this.Exit();
                }
                if(keyboard.IsKeyClicked(Keys.A))
                {
                    this.camera.IncZoom();
                }
                if(keyboard.IsKeyClicked(Keys.Z)) 
                { 
                this.camera.DecZoom();
                }

            }

            if(sampleTimer.Elapsed.TotalSeconds>1d)
            {
                this.bodyCountString = Math.Round(this.totalBodyCount /(double) this.totalSampleCount, 4).ToString();
                this.worldStepTimeString = Math.Round(this.totalWorldStepTime / (double)this.totalSampleCount, 4).ToString();
                this.totalBodyCount = 0;
                this.totalWorldStepTime = 0;
                this.totalSampleCount = 0;
                this.sampleTimer.Restart();
            }

            this.watch.Restart();
            this.world.Step(FlatUtil.GetElapsedTimeInSeconds(gameTime),20);
            this.watch.Stop();

            this.totalWorldStepTime += this.watch.Elapsed.TotalMilliseconds;
            this.totalBodyCount += this.world.BodyCount;
            this.totalSampleCount++;

            this.camera.GetExtents(out _, out _, out float viewBottom, out _ );

            this.entityRemovalList.Clear();

           for(int i =0; i<this.entityList.Count; i++)
            {
                FlatEntity entity = this.entityList[i];
                FlatBody body = entity.Body;

                if(body.IsStatic)
                {
                    continue;
                }
                FlatAABB box = body.GetAABB();

                if(box.Max.Y < viewBottom)
                {
                    this.entityRemovalList.Add(entity);
                }
            }

           for(int i =0; i<this.entityRemovalList.Count; i++)
            {
                FlatEntity entity = entityRemovalList[i];
                this.world.RemoveBody(entity.Body);
                this.entityList.Remove(entity);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            this.GraphicsDevice.Clear(new Color(50,60,70));


            this.shapes.Begin(this.camera);
            for(int i=0; i<this.entityList.Count; i++)
            {
                this.entityList[i].Draw(this.shapes);
            }

            this.shapes.End();




            this.screen.Unset();
            this.screen.Present(this.sprites);

            base.Draw(gameTime);
        
     
        }
    }
}