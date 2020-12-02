using System;
using ConsoleEngine;
using ConsoleEngine.Models;

namespace ConsoleUI
{
    public class ThreeDMaze : ConsoleEngine.ConsoleEngine
    {
        private double _playerX = 8;
        private double _playerY = 8;
        private double _playerA = 0;

        private const int _mapHeight = 16;
        private const int _mapWidth = 16;

        private const double _FOV = Math.PI / 4;
        private const int _depth = 16;

        private string _map = string.Empty;

        protected override bool OnUserCreate()
        {
            _map += "################";
            _map += "#..............#";
            _map += "#..............#";
            _map += "#..............#";
            _map += "#.........#....#";
            _map += "#.........#....#";
            _map += "#..............#";
            _map += "#..............#";
            _map += "#..............#";
            _map += "#..............#";
            _map += "#..............#";
            _map += "#..............#";
            _map += "#.......########";
            _map += "#..............#";
            _map += "#..............#";
            _map += "################";

            return true;
        }

        protected override bool OnUserUpdate(long elapsedTime)
        {
            // game timing
            double speedFactor = (double) elapsedTime / TicksPerMillisecond / 200;

            // controls
            MovePlayer(speedFactor);

            // output
            DrawWalls();

            DrawText((elapsedTime / (double) TicksPerMillisecond).ToString(), 5, 5, AnsiColor.Red);

            return true;
        }

        private void DrawWalls()
        {
            // x => loop trough screen columns, left to right
            for (int x = 0; x < ScreenWidth; x++) {
                // For each column, calculate the projected ray angle into world space
                double rayAngle = _playerA - _FOV / 2 + x / (double) ScreenWidth * _FOV;
                double distanceToWall = 0;
                bool hitWall = false;

                double eyeX = Math.Sin(rayAngle); // unit vector for ray in player space
                double eyeY = Math.Cos(rayAngle);

                while (!hitWall && distanceToWall < _depth) {
                    distanceToWall += 0.1;

                    int testX = (int) (_playerX + eyeX * distanceToWall);
                    int testY = (int) (_playerY + eyeY * distanceToWall);

                    // test if ray is out of bounds
                    if (testX < 0 || testX >= _mapWidth || testY < 0 || testY > _mapHeight) {
                        hitWall = true; // just set distance to maximum depth
                        distanceToWall = _depth;
                    } else {
                        // Ray is inbounds so test to see if the ray cell is a wall block
                        if (_map[testY * _mapWidth + testX] == '#') {
                            hitWall = true;
                        }
                    }
                }

                int ceiling = (int) (ScreenHeight / 2.0 - ScreenHeight / distanceToWall);
                int floor = ScreenHeight - ceiling;

                for (int y = 0; y < ScreenHeight; y++) {
                    if (y < ceiling) {
                        DrawText(' ', x, y);
                    } else if (y >= ceiling && y <= floor) {
                        DrawText(GetWallShade(distanceToWall), x, y);
                    } else {
                        DrawText(GetFloorShade(y), x, y);
                    }
                }
            }
        }

        private char GetFloorShade(int y)
        {
            char shade;
            double b = 1.0 - (y - ScreenHeight / 2.0) / (ScreenHeight / 2.0);
            if (b < 0.25) {
                shade = '#';
            } else if (b < 0.5) {
                shade = 'x';
            } else if (b < 0.75) {
                shade = '-';
            } else if (b < 0.9) {
                shade = '.';
            } else {
                shade = ' ';
            }

            return shade;
        }

        private static char GetWallShade(double distanceToWall)
        {
            char shade;

            if (distanceToWall <= _depth / 4.0) {
                shade = (char) PixelType.Solid;
            } else if (distanceToWall <= _depth / 3.0) {
                shade = (char) PixelType.ThreeQuarters;
            } else if (distanceToWall <= _depth / 2.0) {
                shade = (char) PixelType.Half;
            } else if (distanceToWall <= _depth) {
                shade = (char) PixelType.Quarter;
            } else {
                shade = ' ';
            }

            return shade;
        }

        private void MovePlayer(double speedFactor)
        {
            if (NativeKeyboard.IsKeyDown(KeyCode.A)) {
                _playerA -= 0.5 * speedFactor;
            }

            if (NativeKeyboard.IsKeyDown(KeyCode.D)) {
                _playerA += 0.5 * speedFactor;
            }

            if (NativeKeyboard.IsKeyDown(KeyCode.W)) {
                double x = Math.Sin(_playerA) * 2.0 * speedFactor;
                double y = Math.Cos(_playerA) * 2.0 * speedFactor;

                if (_map[(int) (_playerY + y) * _mapWidth + (int) (_playerX + x)] != '#') {
                    _playerX += x;
                    _playerY += y;
                }
            }

            if (NativeKeyboard.IsKeyDown(KeyCode.S)) {
                double x = Math.Sin(_playerA) * 2.0 * speedFactor;
                double y = Math.Cos(_playerA) * 2.0 * speedFactor;

                if (_map[(int) (_playerY - y) * _mapWidth + (int) (_playerX - x)] != '#') {
                    _playerX -= x;
                    _playerY -= y;
                }
            }
        }
    }
}