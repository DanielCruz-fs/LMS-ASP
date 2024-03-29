﻿using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private readonly LibraryContext context;

        public LibraryAssetService(LibraryContext context)
        {
            this.context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            this.context.Add(newAsset);
            this.context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return this.context.LibraryAssets.Include(asset => asset.Status)
                                             .Include(asset => asset.Location)
                                             .ToList();
        }

        public LibraryAsset GetById(int id)
        {
            return this.context.LibraryAssets.Include(asset => asset.Status)
                                             .Include(asset => asset.Location)
                                             .FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return this.context.LibraryAssets.FirstOrDefault(asset => asset.Id == id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (this.context.Books.Any(book => book.Id == id))
            {
                return this.context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            return "";
        }

        public string GetIsbn(int id)
        {
            if (this.context.Books.Any(book => book.Id == id))
            {
                return this.context.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }
            return "";
        }

        public string GetTitle(int id)
        {
            return this.context.Books.FirstOrDefault(book => book.Id == id).Title;
        }

        public string GetType(int id)
        {
            var book = this.context.LibraryAssets.OfType<Book>().Where(b => b.Id == id);
            return book.Any() ? "Book" : "Video";
        }
        public string GetAuthorOrDirector(int id)
        {
            var isBook = this.context.LibraryAssets.OfType<Book>()
                                                   .Where(asset => asset.Id == id).Any();
            var isVideo = this.context.LibraryAssets.OfType<Video>()
                                                   .Where(asset => asset.Id == id).Any();
            return isBook ?
                this.context.Books.FirstOrDefault(book => book.Id == id).Author :
                this.context.Videos.FirstOrDefault(video => video.Id == id).Director
                ?? "Unknown";
        }
    }
}
