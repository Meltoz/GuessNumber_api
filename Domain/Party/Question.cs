using Domain.Enums;
using Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Domain.Party
{
    public class Question
    {
        public Guid Id { get; private set; } = Guid.Empty;

        public Libelle Libelle { get; private set; }

        public string Response { get; private set; }

        public Category Category { get; private set; }

        public Author? Author { get; private set; }

        public Unit? Unit { get; private set; }

        public VisibilityQuestion Visibility { get; private set; }

        public TypeQuestion Type { get; private set; }

        private Question()
        {

        }

        public Question(
            string libelle,
            string response,
            Category category,
            VisibilityQuestion visibility,
            TypeQuestion type,
            string? author, 
            string? unit)
        {
            ChangeLibelle(libelle);
            ChangeResponse(response);
            ChangeCategory(category);
            SetVisibilityAndType(visibility, type);

            if (!string.IsNullOrWhiteSpace(author))
                ChangeAuthor(author);

            if(!string.IsNullOrWhiteSpace(unit))
                ChangeUnit(unit);
        }
        public Question(
            Guid id,
           string libelle,
           string response,
           Category category,
           VisibilityQuestion visibilty,
           TypeQuestion type, 
           string? author,
           string? unit) : this(libelle, response, category, visibilty, type, author, unit)
        {
            if(Id == Guid.Empty && id != Guid.Empty)
            {
                Id = id;
            }
        }

        public void ChangeLibelle(string newLibelle)
        {
            if (string.IsNullOrWhiteSpace(newLibelle))
                throw new ArgumentException("Libelle must be set");

            if (newLibelle.EndsWith("?"))
            {
                newLibelle = newLibelle.Substring(0, newLibelle.Length - 1);
            }

            Libelle = Libelle.Create(newLibelle);
        }

        public void ChangeResponse(string newResponse)
        {
            if (string.IsNullOrWhiteSpace(newResponse))
                throw new ArgumentException("Response can't be empty");

            string patternValidResponse = @"^\d+$";
            if (!Regex.IsMatch(newResponse, patternValidResponse))
                throw new ArgumentException("Response is number only");

            Response = newResponse;
        }

        public void ChangeCategory(Category newCategory)
        {
            Category = newCategory;
        }

        public void ChangeVisibility(VisibilityQuestion newVisibility)
        {
            SetVisibilityAndType(newVisibility, Type);
        }

        public void ChangeType(TypeQuestion newType)
        {
            SetVisibilityAndType(Visibility, newType); 
        }

        public void ChangeAuthor(string newAuthor)
        {
            Author = Author.Create(newAuthor);
        }

        public void ChangeUnit(string unit)
        {
            Unit = Unit.Create(unit);
        }

        private void SetVisibilityAndType(VisibilityQuestion visibility, TypeQuestion type)
        {
            bool isMinigameVisible = visibility.HasFlag(VisibilityQuestion.Minigame);
            bool hasMinigameType =
                type.HasFlag(TypeQuestion.PileDansLeMille) ||
                type.HasFlag(TypeQuestion.SurLaPiste) ||
                type.HasFlag(TypeQuestion.UnDernierCoup);

            // Validation stricte
            if (isMinigameVisible && !hasMinigameType)
                throw new InvalidOperationException("Visibility is defined to Minigame, type must include  PileDansLeMille, SurLaPiste or UnDernierCoup.");
            if (!isMinigameVisible && hasMinigameType)
                throw new InvalidOperationException("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame");

            Visibility = visibility;
            Type = type;
        }
    }
}

