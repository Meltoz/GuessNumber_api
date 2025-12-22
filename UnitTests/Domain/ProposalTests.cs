using Domain;

namespace UnitTests.Domain
{
    public class ProposalTests
    {
        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateProposal()
        {
            // Arrange
            var libelle = "Quelle est la capitale de la France";
            var response = "123";
            string? source = null;
            string? author = null;

            // Act
            var proposal = new Proposal(libelle, response, source, author);

            // Assert
            Assert.Equal(Guid.Empty, proposal.Id);
            Assert.Equal(libelle, proposal.Libelle);
            Assert.Equal(response, proposal.Response);
            Assert.Null(proposal.Source);
            Assert.Null(proposal.Author);
        }

        [Fact]
        public void Constructor_WithAuthor_ShouldCreateProposalWithAuthor()
        {
            // Arrange
            var libelle = "Combien font deux plus deux";
            var response = "4";
            string? source = null;
            var author = "Jean Dupont";

            // Act
            var proposal = new Proposal(libelle, response, source, author);

            // Assert
            Assert.Equal(author, proposal.Author);
        }

        [Fact]
        public void Constructor_WithSource_ShouldCreateProposalWithSource()
        {
            // Arrange
            var libelle = "Quelle est la vitesse de la lumière";
            var response = "299792458";
            var source = "https://www.example.com/source";
            string? author = null;

            // Act
            var proposal = new Proposal(libelle, response, source, author);

            // Assert
            Assert.Equal(source, proposal.Source);
        }

        [Fact]
        public void Constructor_WithAuthorAndSource_ShouldCreateCompleteProposal()
        {
            // Arrange
            var libelle = "Quelle est la masse de la Terre";
            var response = "5972";
            var source = "https://www.nasa.gov/earth";
            var author = "Marie Curie";

            // Act
            var proposal = new Proposal(libelle, response, source, author);

            // Assert
            Assert.Equal(author, proposal.Author);
            Assert.Equal(source, proposal.Source);
        }

        [Fact]
        public void Constructor_WithIdAndValidParameters_ShouldCreateProposalWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var libelle = "Proposal avec un ID";
            var response = "42";
            string? source = null;
            string? author = null;

            // Act
            var proposal = new Proposal(id, libelle, response, source, author);

            // Assert
            Assert.Equal(id, proposal.Id);
            Assert.Equal(libelle, proposal.Libelle);
            Assert.Equal(response, proposal.Response);
        }

        [Fact]
        public void Constructor_WithEmptyGuidId_ShouldKeepEmptyGuid()
        {
            // Arrange
            var id = Guid.Empty;
            var libelle = "Proposal test avec ID vide";
            var response = "123";
            string? source = null;
            string? author = null;

            // Act
            var proposal = new Proposal(id, libelle, response, source, author);

            // Assert
            Assert.Equal(Guid.Empty, proposal.Id);
        }

        [Fact]
        public void Constructor_WithEmptyAuthor_ShouldNotSetAuthor()
        {
            // Arrange
            var libelle = "Question test avec auteur vide";
            var response = "123";
            string? source = null;
            var author = "";

            // Act
            var proposal = new Proposal(libelle, response, source, author);

            // Assert
            Assert.Null(proposal.Author);
        }

        [Fact]
        public void Constructor_WithEmptySource_ShouldNotSetSource()
        {
            // Arrange
            var libelle = "Question test avec source vide";
            var response = "123";
            var source = "";
            string? author = null;

            // Act
            var proposal = new Proposal(libelle, response, source, author);

            // Assert
            Assert.Null(proposal.Source);
        }

        #endregion

        #region ChangeLibelle Tests - Cas Nominaux

        [Fact]
        public void ChangeLibelle_WithValidLibelle_ShouldUpdateLibelle()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = "Nouveau libellé valide avec trois mots";

            // Act
            proposal.ChangeLibelle(newLibelle);

            // Assert
            Assert.Equal(newLibelle, proposal.Libelle);
        }

        [Fact]
        public void ChangeLibelle_WithSameLibelle_ShouldNotChangeLibelle()
        {
            // Arrange
            var libelle = "Question test avec libellé identique";
            var proposal = new Proposal(libelle, "123", null, null);

            // Act
            proposal.ChangeLibelle(libelle);

            // Assert
            Assert.Equal(libelle, proposal.Libelle);
        }

        [Fact]
        public void ChangeLibelle_WithLongValidLibelle_ShouldUpdateLibelle()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = "Ceci est un très long libellé qui contient beaucoup de mots pour tester la validation des libellés longs mais valides";

            // Act
            proposal.ChangeLibelle(newLibelle);

            // Assert
            Assert.Equal(newLibelle, proposal.Libelle);
        }

        #endregion

        #region ChangeResponse Tests - Cas Nominaux

        [Fact]
        public void ChangeResponse_WithValidNumericResponse_ShouldUpdateResponse()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "789";

            // Act
            proposal.ChangeResponse(newResponse);

            // Assert
            Assert.Equal(newResponse, proposal.Response);
        }

        [Fact]
        public void ChangeResponse_WithZero_ShouldUpdateResponse()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "0";

            // Act
            proposal.ChangeResponse(newResponse);

            // Assert
            Assert.Equal(newResponse, proposal.Response);
        }

        [Fact]
        public void ChangeResponse_WithLargeNumber_ShouldUpdateResponse()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "999999999";

            // Act
            proposal.ChangeResponse(newResponse);

            // Assert
            Assert.Equal(newResponse, proposal.Response);
        }

        #endregion

        #region ChangeAuthor Tests - Cas Nominaux

        [Fact]
        public void ChangeAuthor_WithValidAuthor_ShouldUpdateAuthor()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newAuthor = "Nouvel Auteur";

            // Act
            proposal.ChangeAuthor(newAuthor);

            // Assert
            Assert.Equal(newAuthor, proposal.Author);
        }

        [Fact]
        public void ChangeAuthor_WithMinimumLength_ShouldUpdateAuthor()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newAuthor = "Max";

            // Act
            proposal.ChangeAuthor(newAuthor);

            // Assert
            Assert.Equal(newAuthor, proposal.Author);
        }

        [Fact]
        public void ChangeAuthor_WithMaximumLength_ShouldUpdateAuthor()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newAuthor = new string('A', 50);

            // Act
            proposal.ChangeAuthor(newAuthor);

            // Assert
            Assert.Equal(newAuthor, proposal.Author);
        }

        #endregion

        #region ChangeSource Tests - Cas Nominaux

        [Fact]
        public void ChangeSource_WithValidHttpSource_ShouldUpdateSource()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "http://www.example.com";

            // Act
            proposal.ChangeSource(newSource);

            // Assert
            Assert.Equal(newSource, proposal.Source);
        }

        [Fact]
        public void ChangeSource_WithValidHttpsSource_ShouldUpdateSource()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "https://www.example.com";

            // Act
            proposal.ChangeSource(newSource);

            // Assert
            Assert.Equal(newSource, proposal.Source);
        }

        [Fact]
        public void ChangeSource_WithLongUrl_ShouldUpdateSource()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "https://www.example.com/very/long/path/to/resource";

            // Act
            proposal.ChangeSource(newSource);

            // Assert
            Assert.Equal(newSource, proposal.Source);
        }

        #endregion

        #region Helper Methods

        private Proposal CreateValidProposal()
        {
            return new Proposal(
                "Proposition test avec trois mots",
                "123",
                null,
                null);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs

        [Fact]
        public void Constructor_WithNullLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            string libelle = null;
            var response = "123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("newLibelle", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "";
            var response = "123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("newLibelle", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespaceLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "   ";
            var response = "123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("newLibelle", exception.Message);
        }

        [Fact]
        public void Constructor_WithTooShortLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "ab";
            var response = "123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Libelle must be > 3 characters", exception.Message);
        }

        [Fact]
        public void Constructor_WithTooLongLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = new string('a', 201);
            var response = "123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Libelle must be <200 characters", exception.Message);
        }

        [Fact]
        public void Constructor_WithLibelleWithTwoWords_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Deux mots";
            var response = "123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Libelle must contains > 3 words", exception.Message);
        }

        [Fact]
        public void Constructor_WithLibelleWithOneWord_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "UnSeulMotTresLong";
            var response = "123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Libelle must contains > 3 words", exception.Message);
        }

        [Fact]
        public void Constructor_WithNonNumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test avec trois mots";
            var response = "abc";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Response is not an number", exception.Message);
        }

        [Fact]
        public void Constructor_WithAlphanumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test avec trois mots";
            var response = "123abc";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Response is not an number", exception.Message);
        }

        [Fact]
        public void Constructor_WithDecimalResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test avec trois mots";
            var response = "12.5";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Response is not an number", exception.Message);
        }

        [Fact]
        public void Constructor_WithNegativeResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var libelle = "Question test avec trois mots";
            var response = "-123";
            string? source = null;
            string? author = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Proposal(libelle, response, source, author));
            Assert.Equal("Response is not an number", exception.Message);
        }

        #endregion

        #region ChangeLibelle Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeLibelle_WithNullLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            string newLibelle = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeLibelle(newLibelle));
            Assert.Equal("newLibelle", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithEmptyLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeLibelle(newLibelle));
            Assert.Equal("newLibelle", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithWhitespaceLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeLibelle(newLibelle));
            Assert.Equal("newLibelle", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithTooShortLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = "ab";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeLibelle(newLibelle));
            Assert.Equal("Libelle must be > 3 characters", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithTooLongLibelle_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = new string('a', 201);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeLibelle(newLibelle));
            Assert.Equal("Libelle must be <200 characters", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithLibelleWithTwoWords_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = "Deux mots";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeLibelle(newLibelle));
            Assert.Equal("Libelle must contains > 3 words", exception.Message);
        }

        [Fact]
        public void ChangeLibelle_WithLibelleWithOneWord_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newLibelle = "UnSeulMotTresLong";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeLibelle(newLibelle));
            Assert.Equal("Libelle must contains > 3 words", exception.Message);
        }

        #endregion

        #region ChangeResponse Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeResponse_WithNonNumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "abc";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeResponse(newResponse));
            Assert.Equal("Response is not an number", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithAlphanumericResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "123abc";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeResponse(newResponse));
            Assert.Equal("Response is not an number", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithDecimalResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "12.5";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeResponse(newResponse));
            Assert.Equal("Response is not an number", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithNegativeResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "-123";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeResponse(newResponse));
            Assert.Equal("Response is not an number", exception.Message);
        }

        [Fact]
        public void ChangeResponse_WithSpacesInResponse_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newResponse = "1 2 3";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeResponse(newResponse));
            Assert.Equal("Response is not an number", exception.Message);
        }

        #endregion

        #region ChangeAuthor Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeAuthor_WithTooShortAuthor_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newAuthor = "ab";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeAuthor(newAuthor));
            Assert.Equal("Author must be >3 characters", exception.Message);
        }

        [Fact]
        public void ChangeAuthor_WithTooLongAuthor_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newAuthor = new string('A', 51);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeAuthor(newAuthor));
            Assert.Equal("Author must be <50 characters", exception.Message);
        }

        [Fact]
        public void ChangeAuthor_WithEmptyString_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newAuthor = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeAuthor(newAuthor));
            Assert.Equal("Author must be >3 characters", exception.Message);
        }

        #endregion

        #region ChangeSource Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeSource_WithTooShortSource_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "ab";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeSource(newSource));
            Assert.Equal("Source must be > 3 characters", exception.Message);
        }

        [Fact]
        public void ChangeSource_WithoutHttpPrefix_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "www.example.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeSource(newSource));
            Assert.Equal("Source is not a website", exception.Message);
        }

        [Fact]
        public void ChangeSource_WithFtpProtocol_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "ftp://example.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeSource(newSource));
            Assert.Equal("Source is not a website", exception.Message);
        }

        [Fact]
        public void ChangeSource_WithEmptyString_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeSource(newSource));
            Assert.Equal("Source must be > 3 characters", exception.Message);
        }

        [Fact]
        public void ChangeSource_WithPlainText_ShouldThrowArgumentException()
        {
            // Arrange
            var proposal = CreateValidProposal();
            var newSource = "example";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => proposal.ChangeSource(newSource));
            Assert.Equal("Source is not a website", exception.Message);
        }

        #endregion
    }
}
