using Domain.Enums;
using Domain.Party;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Domain
{
    public class QuestionTests
    {
        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateQuestion()
        {
            // Arrange
            var libelle = "Quelle est la capitale de la France";
            var response = "123";
            var category = new Category("Géographie");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, null);

            // Assert
            Assert.Equal(Guid.Empty, question.Id);
            Assert.NotNull(question.Libelle);
            Assert.Equal(response, question.Response);
            Assert.Equal(category, question.Category);
            Assert.Equal(visibility, question.Visibility);
            Assert.Equal(type, question.Type);
            Assert.Null(question.Author);
            Assert.Null(question.Unit);
        }

        [Fact]
        public void Constructor_WithAuthor_ShouldCreateQuestionWithAuthor()
        {
            // Arrange
            var libelle = "Combien font 2+2";
            var response = "4";
            var category = new Category("Mathématiques");
            var visibility = VisibilityQuestion.Custom;
            var type = TypeQuestion.Standard;
            var author = "Jean Dupont";

            // Act
            var question = new Question(libelle, response, category, visibility, type, author, null);

            // Assert
            Assert.NotNull(question.Author);
        }

        [Fact]
        public void Constructor_WithUnit_ShouldCreateQuestionWithUnit()
        {
            // Arrange
            var libelle = "Quelle est la vitesse de la lumière";
            var response = "299792458";
            var category = new Category("Physique");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;
            var unit = "m/s";

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, unit);

            // Assert
            Assert.NotNull(question.Unit);
        }

        [Fact]
        public void Constructor_WithAuthorAndUnit_ShouldCreateCompleteQuestion()
        {
            // Arrange
            var libelle = "Quelle est la masse de la Terre";
            var response = "5972";
            var category = new Category("Astronomie");
            var visibility = VisibilityQuestion.Public | VisibilityQuestion.Custom;
            var type = TypeQuestion.Standard;
            var author = "Marie Curie";
            var unit = "10^21 kg";

            // Act
            var question = new Question(libelle, response, category, visibility, type, author, unit);

            // Assert
            Assert.NotNull(question.Author);
            Assert.NotNull(question.Unit);
        }

        [Fact]
        public void Constructor_WithIdAndValidParameters_ShouldCreateQuestionWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var libelle = "Question avec ID";
            var response = "42";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act
            var question = new Question(id, libelle, response, category, visibility, type, null, null);

            // Assert
            Assert.Equal(id, question.Id);
            Assert.NotNull(question.Libelle);
            Assert.Equal(response, question.Response);
        }

        [Fact]
        public void Constructor_WithMinigameVisibilityAndPileDansLeMilleType_ShouldCreateQuestion()
        {
            // Arrange
            var libelle = "Question minigame";
            var response = "100";
            var category = new Category("Minigames");
            var visibility = VisibilityQuestion.Minigame;
            var type = TypeQuestion.PileDansLeMille;

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, null);

            // Assert
            Assert.Equal(visibility, question.Visibility);
            Assert.Equal(type, question.Type);
        }

        [Fact]
        public void Constructor_WithMinigameVisibilityAndSurLaPisteType_ShouldCreateQuestion()
        {
            // Arrange
            var libelle = "Question sur la piste";
            var response = "500";
            var category = new Category("Minigames");
            var visibility = VisibilityQuestion.Minigame;
            var type = TypeQuestion.SurLaPiste;

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, null);

            // Assert
            Assert.Equal(visibility, question.Visibility);
            Assert.Equal(type, question.Type);
        }

        [Fact]
        public void Constructor_WithMinigameVisibilityAndUnDernierCoupType_ShouldCreateQuestion()
        {
            // Arrange
            var libelle = "Question dernier coup";
            var response = "999";
            var category = new Category("Minigames");
            var visibility = VisibilityQuestion.Minigame | VisibilityQuestion.Public;
            var type = TypeQuestion.UnDernierCoup;

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, null);

            // Assert
            Assert.Equal(visibility, question.Visibility);
            Assert.Equal(type, question.Type);
        }

        [Fact]
        public void Constructor_WithLibelleEndingWithQuestionMark_ShouldRemoveQuestionMark()
        {
            // Arrange
            var libelle = "Quelle est la réponse?";
            var response = "42";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, null);

            // Assert
            Assert.NotNull(question.Libelle);
        }

        #endregion

        #region ChangeLibelle Tests - Cas Nominaux

        [Fact]
        public void ChangeLibelle_WithValidLibelle_ShouldUpdateLibelle()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newLibelle = "Nouveau libellé";

            // Act
            question.ChangeLibelle(newLibelle);

            // Assert
            Assert.NotNull(question.Libelle);
        }

        [Fact]
        public void ChangeLibelle_WithLibelleEndingWithQuestionMark_ShouldRemoveQuestionMark()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newLibelle = "Question avec point d'interrogation?";

            // Act
            question.ChangeLibelle(newLibelle);

            // Assert
            Assert.NotNull(question.Libelle);
        }

        #endregion

        #region ChangeResponse Tests - Cas Nominaux

        [Fact]
        public void ChangeResponse_WithValidNumericResponse_ShouldUpdateResponse()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "789";

            // Act
            question.ChangeResponse(newResponse);

            // Assert
            Assert.Equal(newResponse, question.Response);
        }

        [Fact]
        public void ChangeResponse_WithZero_ShouldUpdateResponse()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "0";

            // Act
            question.ChangeResponse(newResponse);

            // Assert
            Assert.Equal(newResponse, question.Response);
        }

        #endregion

        #region ChangeCategory Tests - Cas Nominaux

        [Fact]
        public void ChangeCategory_WithValidCategory_ShouldUpdateCategory()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newCategory = new Category("Nouvelle catégorie");

            // Act
            question.ChangeCategory(newCategory);

            // Assert
            Assert.Equal(newCategory, question.Category);
        }

        #endregion

        #region ChangeVisibility Tests - Cas Nominaux

        [Fact]
        public void ChangeVisibility_WithPublicVisibility_ShouldUpdateVisibility()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newVisibility = VisibilityQuestion.Public;

            // Act
            question.ChangeVisibility(newVisibility);

            // Assert
            Assert.Equal(newVisibility, question.Visibility);
        }

        [Fact]
        public void ChangeVisibility_WithCustomVisibility_ShouldUpdateVisibility()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newVisibility = VisibilityQuestion.Custom;

            // Act
            question.ChangeVisibility(newVisibility);

            // Assert
            Assert.Equal(newVisibility, question.Visibility);
        }

        #endregion

        #region ChangeType Tests - Cas Nominaux

        [Fact]
        public void ChangeType_WithStandardType_ShouldUpdateType()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newType = TypeQuestion.Standard;

            // Act
            question.ChangeType(newType);

            // Assert
            Assert.Equal(newType, question.Type);
        }

        [Fact]
        public void ChangeType_FromStandardToPileDansLeMille_WithMinigameVisibility_ShouldUpdateType()
        {
            // Arrange
            var question = new Question("Test", "123", new Category("Test"),
                VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille, null, null);
            var newType = TypeQuestion.UnDernierCoup;

            // Act
            question.ChangeType(newType);

            // Assert
            Assert.Equal(newType, question.Type);
        }

        #endregion

        #region ChangeAuthor Tests - Cas Nominaux

        [Fact]
        public void ChangeAuthor_WithValidAuthor_ShouldUpdateAuthor()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newAuthor = "Nouvel Auteur";

            // Act
            question.ChangeAuthor(newAuthor);

            // Assert
            Assert.NotNull(question.Author);
        }

        #endregion

        #region ChangeUnit Tests - Cas Nominaux

        [Fact]
        public void ChangeUnit_WithValidUnit_ShouldUpdateUnit()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newUnit = "kg";

            // Act
            question.ChangeUnit(newUnit);

            // Assert
            Assert.NotNull(question.Unit);
        }

        #endregion

        #region Helper Methods

        private Question CreateValidQuestion()
        {
            return new Question(
                "Question test",
                "123",
                new Category("Test"),
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs

        [Fact]
        public void Constructor_WithEmptyLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Libelle must be set", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            string libelle = null;
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Libelle must be set", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespaceLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "   ";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Libelle must be set", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Response can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test";
            string response = null;
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Response can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespaceResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "   ";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Response can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithNonNumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "abc";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void Constructor_WithAlphanumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123abc";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void Constructor_WithDecimalResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "12.5";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void Constructor_WithNegativeResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "-123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void Constructor_WithMinigameVisibilityAndStandardType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Minigame;
            var type = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Visibility is defined to Minigame, type must include  PileDansLeMille, SurLaPiste or UnDernierCoup.", exception.Message);
        }

        [Fact]
        public void Constructor_WithPublicVisibilityAndPileDansLeMilleType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.PileDansLeMille;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame", exception.Message);
        }

        [Fact]
        public void Constructor_WithCustomVisibilityAndSurLaPisteType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Custom;
            var type = TypeQuestion.SurLaPiste;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame", exception.Message);
        }

        [Fact]
        public void Constructor_WithPublicVisibilityAndUnDernierCoupType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.UnDernierCoup;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                new Question(libelle, response, category, visibility, type, null, null));
            Assert.Equal("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyGuidId_ShouldKeepEmptyGuid()
        {
            // Arrange
            var id = Guid.Empty;
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;

            // Act
            var question = new Question(id, libelle, response, category, visibility, type, null, null);

            // Assert
            Assert.Equal(Guid.Empty, question.Id);
        }

        [Fact]
        public void Constructor_WithEmptyAuthor_ShouldNotSetAuthor()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;
            var author = "";

            // Act
            var question = new Question(libelle, response, category, visibility, type, author, null);

            // Assert
            Assert.Null(question.Author);
        }

        [Fact]
        public void Constructor_WithWhitespaceAuthor_ShouldNotSetAuthor()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;
            var author = "   ";

            // Act
            var question = new Question(libelle, response, category, visibility, type, author, null);

            // Assert
            Assert.Null(question.Author);
        }

        [Fact]
        public void Constructor_WithEmptyUnit_ShouldNotSetUnit()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;
            var unit = "";

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, unit);

            // Assert
            Assert.Null(question.Unit);
        }

        [Fact]
        public void Constructor_WithWhitespaceUnit_ShouldNotSetUnit()
        {
            // Arrange
            var libelle = "Question test";
            var response = "123";
            var category = new Category("Test");
            var visibility = VisibilityQuestion.Public;
            var type = TypeQuestion.Standard;
            var unit = "   ";

            // Act
            var question = new Question(libelle, response, category, visibility, type, null, unit);

            // Assert
            Assert.Null(question.Unit);
        }

        #endregion

        #region ChangeLibelle Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeLibelle_WithEmptyLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newLibelle = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeLibelle(newLibelle));
            Assert.Equal("Libelle must be set", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithNullLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            string newLibelle = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeLibelle(newLibelle));
            Assert.Equal("Libelle must be set", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithWhitespaceLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newLibelle = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeLibelle(newLibelle));
            Assert.Equal("Libelle must be set", exception.Message);
        }

        #endregion

        #region ChangeResponse Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeResponse_WithEmptyResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response can't be empty", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithNullResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            string newResponse = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response can't be empty", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithWhitespaceResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response can't be empty", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithNonNumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "abc";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithAlphanumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "123abc";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithDecimalResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "12.5";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithNegativeResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "-123";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response is number only", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithSpacesInResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newResponse = "1 2 3";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => question.ChangeResponse(newResponse));
            Assert.Equal("Response is number only", exception.Message);
        }

        #endregion

        #region ChangeVisibility Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeVisibility_ToMinigameWithStandardType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newVisibility = VisibilityQuestion.Minigame;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => question.ChangeVisibility(newVisibility));
            Assert.Equal("Visibility is defined to Minigame, type must include  PileDansLeMille, SurLaPiste or UnDernierCoup.", exception.Message);
        }

        [Fact]
        public void ChangeVisibility_ToPublicWithMinigameType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var question = new Question("Test", "123", new Category("Test"),
                VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille, null, null);
            var newVisibility = VisibilityQuestion.Public;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => question.ChangeVisibility(newVisibility));
            Assert.Equal("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame", exception.Message);
        }

        #endregion

        #region ChangeType Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeType_ToPileDansLeMilleWithPublicVisibility_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newType = TypeQuestion.PileDansLeMille;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => question.ChangeType(newType));
            Assert.Equal("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame", exception.Message);
        }

        [Fact]
        public void ChangeType_ToSurLaPisteWithCustomVisibility_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var question = new Question("Test", "123", new Category("Test"),
                VisibilityQuestion.Custom, TypeQuestion.Standard, null, null);
            var newType = TypeQuestion.SurLaPiste;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => question.ChangeType(newType));
            Assert.Equal("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame", exception.Message);
        }

        [Fact]
        public void ChangeType_ToUnDernierCoupWithPublicVisibility_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var question = CreateValidQuestion();
            var newType = TypeQuestion.UnDernierCoup;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => question.ChangeType(newType));
            Assert.Equal("Type is defined to PileDansLeMille, SurLaPiste or UnDernierCoup. Visibility must include Minigame", exception.Message);
        }

        [Fact]
        public void ChangeType_ToStandardWithMinigameVisibility_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var question = new Question("Test", "123", new Category("Test"),
                VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille, null, null);
            var newType = TypeQuestion.Standard;

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => question.ChangeType(newType));
            Assert.Equal("Visibility is defined to Minigame, type must include  PileDansLeMille, SurLaPiste or UnDernierCoup.", exception.Message);
        }

        #endregion
    }
}
