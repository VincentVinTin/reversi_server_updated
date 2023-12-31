﻿namespace ReversiRestApi.Models.Interfaces
{
    public interface ISpelRepository
    {
        Task AddSpel(Spel spel);

        public List<Spel> GetSpellen();

        Task<Spel?> GetSpel(string spelToken);

        Task UpdateSpel(Spel spel);
    }
}