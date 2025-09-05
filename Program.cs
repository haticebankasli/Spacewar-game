using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
class Program
{
    static void Main()
    {
        Game game = new Game();
        game.StartGame();
    }
}
class Game
{
    private RenderWindow window;
    private Spaceship player;
    private List<Rock> rocks = new List<Rock>();
    private Clock rockSpawnTimer = new Clock();
    private float rockSpawnRate = 2f;
    private Texture rockTexture;
    private int rockDamage = 10;
    private List<Enemy> enemies = new List<Enemy>();
    private List<Bullet> bullets = new List<Bullet>();
    private List<Bullet> enemyBullets = new List<Bullet>();
    private List<PowerUp> powerUps = new List<PowerUp>();
    private Clock enemySpawnTimer = new Clock();
    private Clock enemyShootTimer = new Clock();
    private Clock powerUpSpawnTimer = new Clock();
    private Font font;
    private Text scoreText;
    private Text levelText;
    private Text gameOverText;
    private Text healthText;
    private Text startGameText;
    private Text startButtonText;
    private Text Name;
    private RectangleShape startButton;
    private List<int> topScores = new List<int>();
    private Text topScoresText;
    private Random rand = new Random();
    private List<CircleShape> stars = new List<CircleShape>();
    private int score = 0;
    private int level = 1;
    private bool isGameOver = false; // is game over
    private bool isGameStarted = false;

    public Game()
    {
        window = new RenderWindow(new VideoMode(800, 600), "Spacewar");
        window.Closed += (sender, e) => window.Close();
        window.MouseButtonPressed += HandleMouseClick;
        GenerateStars();
        topScores = LoadScores();
        try
        {
            rockTexture = new Texture("rock.png");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading rock texture: {ex.Message}");
            Environment.Exit(1);
        }
        try
        {
            Texture playerTexture = new Texture("player.png");
            Texture bulletTexture = new Texture("bullet.png");
            player = new Spaceship(playerTexture, bulletTexture);
            font = new Font("arial.ttf");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }

        scoreText = new Text($"Score: {score}", font, 20)
        {
            FillColor = Color.White,
            Position = new Vector2f(10, 10)
        };

        levelText = new Text($"Level: {level}", font, 20)
        {
            FillColor = Color.White,
            Position = new Vector2f(10, 40)
        };

        healthText = new Text($"Health: {player.Health}", font, 20)
        {
            FillColor = Color.White,
            Position = new Vector2f(10, 70)
        };

        gameOverText = new Text("Game Over!", font, 50)
        {
            FillColor = Color.Red,
            Position = new Vector2f(250, 250)
        };
        startGameText = new Text("SPACE GAME", font, 80)
        {
            FillColor = new Color(173, 173, 255),
        };
        float textX = (window.Size.X - startGameText.GetLocalBounds().Width) / 2;
        float textY = (window.Size.Y - startGameText.GetLocalBounds().Height) / 2 - 150;
        startGameText.Position = new Vector2f(textX, textY);

        startButton = new RectangleShape(new Vector2f(200, 50))
        {
            Position = new Vector2f((window.Size.X - 200) / 2, (window.Size.Y + startGameText.GetLocalBounds().Height) / 2),
            FillColor = new Color(173, 173, 255),
            OutlineThickness = 5,
            OutlineColor = new Color(50, 50, 255)

        };
        Name = new Text("HATICE BANKASLI", font, 20)
        {
            FillColor = new Color(192, 192, 192),
        };
        float textU = (window.Size.X - Name.GetLocalBounds().Width) / 2;
        float textA = window.Size.Y - Name.GetLocalBounds().Height - 50;

        Name.Position = new Vector2f(textU, textA);
        startButtonText = new Text("Start Game", font, 25)
        {
            FillColor = Color.Black
        };
        float buttonTextX = startButton.Position.X + (startButton.Size.X - startButtonText.GetLocalBounds().Width) / 2;
        float buttonTextY = startButton.Position.Y + (startButton.Size.Y - startButtonText.GetLocalBounds().Height) / 2;
        startButtonText.Position = new Vector2f(buttonTextX, buttonTextY);
        window.MouseMoved += (sender, e) =>
        {
            if (startButton.GetGlobalBounds().Contains(e.X, e.Y))
            {
                startButton.FillColor = new Color(150, 150, 255);
            }
            else
            {
                startButton.FillColor = new Color(100, 100, 255);
            }
        };
        topScoresText = new Text("", font, 20)
        {
            FillColor = Color.White,
            Position = new Vector2f(600, 10)
        };

        window.Clear();
        window.Draw(startGameText);
        window.Draw(startButton);
        window.Draw(Name);
        window.Draw(startButtonText);
        window.Display();

    }
    private void GenerateStars()
    {
        for (int i = 0; i < 100; i++)
        {
            float x = rand.Next(0, 800);
            float y = rand.Next(0, 600);
            CircleShape star = new CircleShape(1)
            {
                Position = new Vector2f(x, y),
                FillColor = Color.White
            };
            stars.Add(star);
        }
    }
    private void DrawStars()
    {
        foreach (var star in stars)
        {
            window.Draw(star);
        }
    }
    public void StartGame()
    {
        isGameOver = false;
        isGameStarted = false;
        enemies.Clear();
        bullets.Clear();
        enemyBullets.Clear();
        powerUps.Clear();
        score = 0;
        level = 1;
        player.Reset();
        Run();
    }
    private void Run()
    {
        Clock clock = new Clock();

        while (window.IsOpen)
        {
            window.DispatchEvents();
            float deltaTime = clock.Restart().AsSeconds();

            if (!isGameStarted)
            {
                ShowStartScreen();
            }
            else if (!isGameOver)
            {
                HandleInput(deltaTime);
                Update(deltaTime);
                Render();
            }
            else
            {
                EndGame();
            }
        }
    }
    private void ShowStartScreen()
    {

        window.Clear();
        DrawStars();
        window.Draw(startGameText);
        window.Draw(startButton);
        window.Draw(Name);
        window.Draw(startButtonText);
        window.Display();
    }
    private void HandleMouseClick(object sender, MouseButtonEventArgs e)
    {
        if (!isGameStarted && startButton.GetGlobalBounds().Contains(e.X, e.Y))
        {
            isGameStarted = true;
        }
    }
    private void HandleInput(float deltaTime)
    {
        Vector2i mousePosition = Mouse.GetPosition(window);
        float speed = 5f;
        Vector2f targetPosition = new Vector2f(mousePosition.X, mousePosition.Y);
        Vector2f direction = targetPosition - player.Sprite.Position;

        player.Sprite.Position += direction * speed * deltaTime;

        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            player.Shoot(bullets);
        }
        player.Sprite.Position = new Vector2f(
        Math.Clamp(player.Sprite.Position.X, 0, window.Size.X - player.Sprite.GetGlobalBounds().Width),
        Math.Clamp(player.Sprite.Position.Y, 0, window.Size.Y - player.Sprite.GetGlobalBounds().Height)
    );
    }
    private void Update(float deltaTime)
    {
        if (rockSpawnTimer.ElapsedTime.AsSeconds() > rockSpawnRate)
        {
            SpawnRock();
            rockSpawnTimer.Restart();
        }
        foreach (var rock in rocks)
        {
            rock.Move(deltaTime);

            if (CollisionDetector.CheckCollision(rock.Sprite, player.Sprite))
            {
                rock.IsActive = false;
                player.TakeDamage(rockDamage);
                if (player.Health <= 0)
                {
                    isGameOver = true;
                }
            }
        }
        rocks.RemoveAll(r => !r.IsActive);
        if (enemySpawnTimer.ElapsedTime.AsSeconds() > Math.Max(1.5f - level * 0.1f, 0.3f))
        {
            SpawnEnemy();
            enemySpawnTimer.Restart();
        }
        if (enemyShootTimer.ElapsedTime.AsSeconds() > 1.5f)
        {
            foreach (var enemy in enemies)
            {
                enemy.Shoot(enemyBullets);
            }
            enemyShootTimer.Restart();
        }
        if (powerUpSpawnTimer.ElapsedTime.AsSeconds() > 10.0f)
        {
            SpawnPowerUp();
            powerUpSpawnTimer.Restart();
        }
        foreach (var bullet in bullets)
        {
            bullet.Move(-10);
        }
        foreach (var bullet in enemyBullets)
        {
            bullet.Move(5);
        }
        foreach (var enemy in enemies)
        {
            enemy.Move(deltaTime);
            if (enemy.Sprite.Position.Y > window.Size.Y)
            {
                enemy.Destroy();
                score = Math.Max(0, score - 10);
            }
        }
        foreach (var powerUp in powerUps)
        {
            powerUp.Move(0.1f);
        }
        float spawnRate = Math.Max(1.5f - level * 0.1f, 0.2f);
        int numEnemiesToSpawn = 1 + level / 2;
        int maxEnemies = 15 + level * 2;

        if (enemies.Count < maxEnemies && enemySpawnTimer.ElapsedTime.AsSeconds() > spawnRate)
        {
            for (int i = 0; i < numEnemiesToSpawn; i++)
            {
                SpawnEnemy();
            }
            enemySpawnTimer.Restart();
        }
        CheckCollisions();
        bullets.RemoveAll(b => b.Sprite.Position.Y < 0 || !b.IsActive);
        enemyBullets.RemoveAll(b => b.Sprite.Position.Y > 600 || !b.IsActive);
        enemies.RemoveAll(e => e.Destroyed);
        powerUps.RemoveAll(p => p.Collected);

        if (score >= level * 100)
        {
            level++;
            levelText.DisplayedString = $"Level: {level}";
        }
        scoreText.DisplayedString = $"Score: {score}";
        healthText.DisplayedString = $"Health: {player.Health}";
    }
    private void Render() //draw the graphical element
    {
        window.Clear();
        DrawStars();
        player.Draw(window);
        bullets.ForEach(b => b.Draw(window));
        enemyBullets.ForEach(b => b.Draw(window));
        enemies.ForEach(e => e.Draw(window));
        powerUps.ForEach(p => p.Draw(window));
        rocks.ForEach(r => r.Draw(window));
        window.Draw(scoreText);
        window.Draw(levelText);
        window.Draw(healthText);
        window.Draw(topScoresText);
        window.Display();
    }
    private void EndGame()
    {
        topScores.Add(score);
        topScores.Sort((a, b) => b.CompareTo(a));
        if (topScores.Count > 5)
        {
            topScores.RemoveAt(5);
        }
        SaveScores(topScores);
        ScoreBoard scoreBoard = new ScoreBoard(new List<int>(topScores));
        scoreBoard.Show();
        isGameStarted = false;
        isGameOver = false;
        score = 0;
        level = 1;
        player.Reset();

    }
    private void SpawnRock()
    {
        int side = rand.Next(0, 4);
        float x = 0;
        float y = 0;
        float speedX = 0;
        float speedY = 0;
        float rockSpeed = 100f;

        switch (side)
        {
            case 0:
                x = rand.Next(0, (int)window.Size.X);
                y = -rockTexture.Size.Y;
                speedY = rockSpeed;
                break;
            case 1:
                x = window.Size.X;
                y = rand.Next(0, (int)window.Size.Y);
                speedX = -rockSpeed;
                break;
            case 2:
                x = rand.Next(0, (int)window.Size.X);
                y = window.Size.Y;
                speedY = -rockSpeed;
                break;
            case 3:
                x = -rockTexture.Size.X;
                y = rand.Next(0, (int)window.Size.Y);
                speedX = rockSpeed;
                break;
        }
        rocks.Add(new Rock(rockTexture, new Vector2f(x, y), new Vector2f(speedX, speedY)));
    }
    private List<int> LoadScores()
    {
        var scores = new List<int>();
        string scoresFilePath = "scores.txt";

        if (File.Exists(scoresFilePath))
        {
            try
            {
                scores = File.ReadAllLines(scoresFilePath)
                    .Where(line => int.TryParse(line, out _))
                    .Select(int.Parse)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading scores: {ex.Message}");
            }
        }
        return scores;
    }
    private void SaveScores(List<int> scores)
    {
        string scoresFilePath = "scores.txt";
        try
        {
            File.WriteAllLines(scoresFilePath, scores.Select(score => score.ToString()));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving scores: {ex.Message}");
        }
    }
    private void SpawnEnemy()
    {
        Texture basicEnemyTexture = new Texture("enemy.png");
        Texture fastEnemyTexture = new Texture("fast.png");
        Texture strongEnemyTexture = new Texture("strong.png");
        Texture bossEnemyTexture = new Texture("Boss.png");
        int x = rand.Next(50, 750);
        bool bossExists = enemies.Any(e => e is BossEnemy);
        int bossSpawnChance = Math.Min(level * 10, 80);
        if (level >= 3 && !bossExists && rand.Next(0, 100) < bossSpawnChance)
        {
            enemies.Add(new BossEnemy(bossEnemyTexture, x, 0, player, window));
            return;
        }
        int enemyType = rand.Next(0, 10);
        if (enemyType < 3)
        {
            enemies.Add(new BasicEnemy(basicEnemyTexture, x, 0, player));
        }
        else if (enemyType < 6)
        {
            enemies.Add(new FastEnemy(fastEnemyTexture, x, 0, player));
        }
        else if (enemyType < 9)
        {
            enemies.Add(new StrongEnemy(strongEnemyTexture, x, 0, player));
        }

    }
    private void SpawnPowerUp()
    {
        Texture powerUpTexture = new Texture("powerup.png");
        powerUps.Add(new PowerUp(powerUpTexture, new Vector2f(rand.Next(0, 800), 0)));
    }
    private void CheckCollisions()
    {
        foreach (var bullet in bullets)
        {
            foreach (var enemy in enemies)
            {
                if (CollisionDetector.CheckCollision(bullet.Sprite, enemy.Sprite))
                {
                    bullet.IsActive = false;
                    enemy.TakeDamage(player.BulletDamage);

                    if (enemy.Health <= 0)
                    {
                        enemy.Destroy();
                        score += 10;
                    }
                }
            }
        }
        foreach (var bullet in enemyBullets)
        {
            if (CollisionDetector.CheckCollision(bullet.Sprite, player.Sprite))
            {
                bullet.IsActive = false;
                player.TakeDamage(2); //her bullet için 2 damage yapıyor

                if (player.Health <= 0)
                {
                    isGameOver = true;
                }
            }
        }
        foreach (var enemy in enemies)
        {
            if (CollisionDetector.CheckCollision(enemy.Sprite, player.Sprite))
            {
                enemy.Destroy();
                player.TakeDamage(enemy.Damage);

                if (player.Health <= 0)
                {
                    isGameOver = true;
                }
            }
        }
        foreach (var powerUp in powerUps)
        {
            if (CollisionDetector.CheckCollision(powerUp.Sprite, player.Sprite))
            {
                powerUp.Collected = true;
                ApplyPowerUpEffect(powerUp.Type);
            }
        }

    }
    private void ApplyPowerUpEffect(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Health:
                player.Health = Math.Min(player.Health + 20, 100);
                break;

            case PowerUpType.Damage:
                player.BulletDamage += 5;
                break;
        }
    }
}
public class Rock
{
    public Sprite Sprite { get; private set; }
    public bool IsActive { get; set; } = true;
    public Vector2f Velocity { get; set; }

    public Rock(Texture texture, Vector2f position, Vector2f velocity)
    {
        Sprite = new Sprite(texture)
        {
            Position = position
        };
        Velocity = velocity;
    }

    public void Move(float deltaTime)
    {
        Sprite.Position += Velocity * deltaTime;
    }

    public void Draw(RenderWindow window)
    {
        if (IsActive)
        {
            window.Draw(Sprite);
        }
    }
}
class Spaceship
{
    public Sprite Sprite { get; private set; }
    public int Health { get; set; } = 100;
    public int BulletDamage { get; set; } = 10;
    private Texture bulletTexture;

    private Clock shootClock = new Clock();
    private float shootDelay = 0.005f;
    public Spaceship(Texture texture, Texture bulletTexture)
    {
        Sprite = new Sprite(texture)
        {
            Position = new Vector2f(400, 500)
        };
        this.bulletTexture = bulletTexture;
    }
    public void Shoot(List<Bullet> bullets)
    {
        if (shootClock.ElapsedTime.AsSeconds() >= shootDelay)
        {
            bullets.Add(new Bullet(bulletTexture, new Vector2f(Sprite.Position.X + 15, Sprite.Position.Y)));
            shootClock.Restart();
        }
    }
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0)
        {
            Health = 0;
        }
    }
    public void Reset()
    {
        Health = 100;
        Sprite.Position = new Vector2f(400, 500);
    }
    public void Draw(RenderWindow window)
    {
        window.Draw(Sprite);
    }
}
public class Bullet
{
    public Sprite Sprite { get; private set; }
    public bool IsActive { get; set; } = true;
    public Vector2f Direction { get; set; } = new Vector2f(0, 1);

    public Bullet(Texture texture, Vector2f position)
    {
        Sprite = new Sprite(texture)
        {
            Position = position
        };
    }
    public void Move(float speed)
    {
        Sprite.Position += Direction * speed;
    }
    public void Draw(RenderWindow window)
    {
        window.Draw(Sprite);
    }
}
abstract class Enemy
{
    public Sprite Sprite { get; protected set; }
    public int Health { get; protected set; }
    public int Damage { get; protected set; }
    public int Speed { get; protected set; }
    public bool Destroyed { get; set; } = false;
    public Enemy(Texture texture, int spawnX, int spawnY, int health, int speed, int damage)
    {
        Sprite = new Sprite(texture)
        {
            Position = new Vector2f(spawnX, spawnY)
        };
        Health = health;
        Speed = speed;
        Damage = damage;
    }
    public abstract void Move(float deltaTime);
    public void TakeDamage(int amount)
    {
        Health -= amount;
    }
    public void Destroy()
    {
        if (!Destroyed)
        {
            Destroyed = true;
            Sprite.Position = new Vector2f(-100, -100);
        }
    }
    public void Draw(RenderWindow window)
    {
        window.Draw(Sprite);
    }
    public virtual void Shoot(List<Bullet> enemyBullets)
    {
        Texture bulletTexture = new Texture("enemy_bullet.png");
        enemyBullets.Add(new Bullet(bulletTexture, new Vector2f(Sprite.Position.X + 15, Sprite.Position.Y + 30)));
    }
}
class BasicEnemy : Enemy
{
    private Spaceship player;

    public BasicEnemy(Texture texture, int spawnX, int spawnY, Spaceship player)
        : base(texture, spawnX, spawnY, 50, 150, 10)
    {
        this.player = player;
    }
    public override void Move(float deltaTime)
    {
        float playerX = player.Sprite.Position.X;
        float directionX = playerX - Sprite.Position.X;
        float moveX = Math.Clamp(directionX, -Speed * deltaTime, Speed * deltaTime);
        Sprite.Position = new Vector2f(Sprite.Position.X + moveX, Sprite.Position.Y);
    }
}
class FastEnemy : Enemy
{
    private Spaceship player;

    public FastEnemy(Texture texture, int spawnX, int spawnY, Spaceship player)
        : base(texture, spawnX, spawnY, 200, 300, 15)
    {
        this.player = player;
    }

    public override void Move(float deltaTime)
    {
        float playerX = player.Sprite.Position.X;
        float playerY = player.Sprite.Position.Y;
        float directionX = playerX - Sprite.Position.X;
        float directionY = playerY - Sprite.Position.Y;
        float length = (float)Math.Sqrt(directionX * directionX + directionY * directionY);
        if (length != 0)
        {
            directionX /= length;
            directionY /= length;
        }
        float moveX = directionX * Speed * deltaTime;
        float moveY = directionY * Speed * deltaTime;
        Sprite.Position = new Vector2f(Sprite.Position.X + moveX, Sprite.Position.Y + moveY);
    }
}
class StrongEnemy : Enemy
{
    private Spaceship player;
    private Vector2f targetDirection;
    private Clock directionUpdateClock = new Clock();
    private float directionUpdateDelay = 0.3f;

    public StrongEnemy(Texture texture, int spawnX, int spawnY, Spaceship player)
        : base(texture, spawnX, spawnY, 400, 100, 50)
    {
        this.player = player;
        targetDirection = new Vector2f(0, 1);
    }

    public override void Move(float deltaTime)
    {
        if (directionUpdateClock.ElapsedTime.AsSeconds() >= directionUpdateDelay)
        {
            float playerX = player.Sprite.Position.X;
            float playerY = player.Sprite.Position.Y;
            float directionX = playerX - Sprite.Position.X;
            float directionY = playerY - Sprite.Position.Y;
            float length = (float)Math.Sqrt(directionX * directionX + directionY * directionY);
            if (length != 0)
            {
                directionX /= length;
                directionY /= length;
            }
            targetDirection = new Vector2f(directionX, directionY);
            directionUpdateClock.Restart();
        }
        Sprite.Position += targetDirection * Speed * deltaTime;
    }
}
class BossEnemy : Enemy
{
    private Spaceship player;
    private RenderWindow window;

    public BossEnemy(Texture texture, int spawnX, int spawnY, Spaceship player, RenderWindow window)
        : base(texture, spawnX, spawnY, 500, 100, 30)
    {
        this.player = player;
        this.window = window;
    }
    public override void Move(float deltaTime)
    {
        Sprite.Position = new Vector2f(Sprite.Position.X + Speed * deltaTime, Sprite.Position.Y);
        if (Sprite.Position.X > window.Size.X - Sprite.GetLocalBounds().Width || Sprite.Position.X < 0)
        {
            Speed = -Speed;
        }
    }
    public override void Shoot(List<Bullet> enemyBullets)
    {
        Texture bulletTexture = new Texture("enemy_bullet.png");
        Vector2f targetPosition = player.Sprite.Position;
        Vector2f direction = targetPosition - Sprite.Position;
        float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
        direction /= length;
        Bullet homingBullet = new Bullet(bulletTexture, new Vector2f(Sprite.Position.X + Sprite.GetLocalBounds().Width / 2, Sprite.Position.Y + 50));
        homingBullet.Direction = direction;
        enemyBullets.Add(homingBullet);
    }
}
class PowerUp
{
    public Sprite Sprite { get; private set; }
    public PowerUpType Type { get; private set; }
    public bool Collected { get; set; } = false;
    public PowerUp(Texture texture, Vector2f position)
    {
        Sprite = new Sprite(texture)
        {
            Position = position
        };
        Type = (PowerUpType)new Random().Next(0, 2);
    }
    public void Move(float speed)
    {
        Sprite.Position = new Vector2f(Sprite.Position.X, Sprite.Position.Y + speed);
    }
    public void Draw(RenderWindow window)
    {
        window.Draw(Sprite);
    }
}
enum PowerUpType
{
    Health,
    Damage
}
class CollisionDetector
{
    public static bool CheckCollision(Sprite sprite1, Sprite sprite2)
    {
        return sprite1.GetGlobalBounds().Intersects(sprite2.GetGlobalBounds());
    }
}
class ScoreBoard
{
    private RenderWindow window;
    private Font font;
    private Text titleText;
    private Text scoreText;
    private List<int> topScores;
    public ScoreBoard(List<int> scores)
    {
        topScores = scores;
        window = new RenderWindow(new VideoMode(400, 300), "Top 5 Scores", Styles.Close);
        window.Closed += (sender, e) => window.Close();
        try
        {
            font = new Font("arial.ttf");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading font: {ex.Message}");
            Environment.Exit(1);
        }

        titleText = new Text("Top 5 Scores", font, 30)
        {
            FillColor = Color.White,
            Position = new Vector2f(100, 20)
        };

        scoreText = new Text("", font, 20)
        {
            FillColor = Color.White,
            Position = new Vector2f(50, 80)
        };
    }

    public void Show()
    {
        while (window.IsOpen)
        {
            window.DispatchEvents();
            Render();
        }
    }
    private void Render()
    {
        window.Clear();

        string scores = "";
        for (int i = 0; i < topScores.Count; i++)
        {
            scores += $"{i + 1}. {topScores[i]}\n";
        }
        scoreText.DisplayedString = scores;

        window.Draw(titleText);
        window.Draw(scoreText);
        window.Display();
    }
}