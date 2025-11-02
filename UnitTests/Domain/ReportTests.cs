using Domain;
using Domain.Enums;

namespace UnitTests.Domain
{
    public class ReportTests
    {
        #region Constructeur par défaut

        [Fact]
        public void Constructor_Default_ShouldInitializeWithEmptyValues()
        {
            // Act
            var report = new Report();

            // Assert
            Assert.Equal(Guid.Empty, report.Id);
            Assert.Equal(string.Empty, report.Explanation);
            Assert.Null(report.Mail);
        }

        #endregion

        #region Constructeur avec paramètres (sans Id)

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateReport()
        {
            // Arrange
            var type = TypeReport.Bug;
            var context = ContextReport.Site;
            var explanation = "Explication valide";
            var mail = "test@example.com";

            // Act
            var report = new Report(type, context, explanation, mail);

            // Assert
            Assert.Equal(type, report.Type);
            Assert.Equal(context, report.Context);
            Assert.Equal(explanation, report.Explanation);
            Assert.Equal(mail, report.Mail);
        }

        [Fact]
        public void Constructor_WithNullMail_ShouldCreateReportWithoutMail()
        {
            // Arrange
            var explanation = "Explication sans mail";

            // Act
            var report = new Report(TypeReport.Improuvment, ContextReport.Question, explanation, null);

            // Assert
            Assert.Null(report.Mail);
            Assert.Equal(explanation, report.Explanation);
        }

        #endregion

        #region Constructeur avec Id

        [Fact]
        public void Constructor_WithValidId_ShouldNotSetId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var explanation = "Explication avec id";

            // Act
            var report = new Report(id, TypeReport.Bug, ContextReport.Site, explanation, null);

            // Assert
            Assert.Equal(id, report.Id);
        }

        [Fact]
        public void Constructor_WithEmptyId_ShouldSetEmptyId()
        {
            // Arrange
            var explanation = "Explication avec id vide";

            // Act
            var report = new Report(Guid.Empty, TypeReport.Bug, ContextReport.Site, explanation, null);

            // Assert
            Assert.Equal(Guid.Empty, report.Id);
        }

        #endregion

        #region ChangeExplanation - Cas nominaux

        [Fact]
        public void ChangeExplanation_WithValidExplanation_ShouldUpdateExplanation()
        {
            // Arrange
            var report = new Report();
            var newExplanation = "Nouvelle explication valide";

            // Act
            report.ChangeExplanation(newExplanation);

            // Assert
            Assert.Equal(newExplanation, report.Explanation);
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("Exactement six caractères")]
        [InlineData("Une très longue explication avec beaucoup de détails")]
        public void ChangeExplanation_WithDifferentValidLengths_ShouldUpdateExplanation(string explanation)
        {
            // Arrange
            var report = new Report();

            // Act
            report.ChangeExplanation(explanation);

            // Assert
            Assert.Equal(explanation, report.Explanation);
        }

        #endregion

        #region ChangeExplanation - Cas d'erreurs

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ChangeExplanation_WithNullOrWhitespace_ShouldThrowArgumentException(string explanation)
        {
            // Arrange
            var report = new Report();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => report.ChangeExplanation(explanation));
            Assert.Equal("explanation", exception.ParamName);
            Assert.Contains("Explanation must be set", exception.Message);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("12345")]
        public void ChangeExplanation_WithLengthLessThanOrEqualTo5_ShouldThrowArgumentException(string explanation)
        {
            // Arrange
            var report = new Report();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => report.ChangeExplanation(explanation));
            Assert.Contains("Explanation must be length > 5", exception.Message);
        }

        #endregion

        #region ChangeExplanation - Cas limites

        [Fact]
        public void ChangeExplanation_WithExactly6Characters_ShouldSucceed()
        {
            // Arrange
            var report = new Report();
            var explanation = "123456"; // Exactement 6 caractères

            // Act
            report.ChangeExplanation(explanation);

            // Assert
            Assert.Equal(explanation, report.Explanation);
        }

        [Fact]
        public void ChangeExplanation_WithWhitespaceAndValidLength_ShouldThrowException()
        {
            // Arrange
            var report = new Report();
            var explanation = "      "; // 6 espaces

            // Act & Assert
            Assert.Throws<ArgumentException>(() => report.ChangeExplanation(explanation));
        }

        #endregion

        #region ChangeMail - Cas nominaux

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.com")]
        [InlineData("first_last@test.co.uk")]
        [InlineData("email123@sub-domain.example.org")]
        public void ChangeMail_WithValidEmail_ShouldUpdateMail(string mail)
        {
            // Arrange
            var report = new Report();

            // Act
            report.ChangeMail(mail);

            // Assert
            Assert.Equal(mail, report.Mail);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangeMail_WithNullOrWhitespace_ShouldNotThrowAndKeepNull(string mail)
        {
            // Arrange
            var report = new Report();

            // Act
            report.ChangeMail(mail);

            // Assert
            Assert.Null(report.Mail);
        }

        #endregion

        #region ChangeMail - Cas d'erreurs

        [Theory]
        [InlineData("invalid.email")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("user@@example.com")]
        [InlineData("user@.com")]
        [InlineData(".user@example.com")]
        [InlineData("user.@example.com")]
        [InlineData("user@example.")]
        [InlineData("user @example.com")]
        [InlineData("user@exam ple.com")]
        public void ChangeMail_WithInvalidEmail_ShouldThrowArgumentException(string mail)
        {
            // Arrange
            var report = new Report();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => report.ChangeMail(mail));
            Assert.Equal("mail", exception.ParamName);
            Assert.Contains("Mail is not valid", exception.Message);
        }

        #endregion

        #region ChangeMail - Cas limites

        [Fact]
        public void ChangeMail_WithMinimumValidEmail_ShouldSucceed()
        {
            // Arrange
            var report = new Report();
            var mail = "a@b.co";

            // Act
            report.ChangeMail(mail);

            // Assert
            Assert.Equal(mail, report.Mail);
        }

        [Fact]
        public void ChangeMail_MultipleChanges_ShouldUpdateCorrectly()
        {
            // Arrange
            var report = new Report();
            var mail1 = "first@example.com";
            var mail2 = "second@example.com";

            // Act
            report.ChangeMail(mail1);
            var firstMail = report.Mail;
            report.ChangeMail(mail2);

            // Assert
            Assert.Equal(mail1, firstMail);
            Assert.Equal(mail2, report.Mail);
        }

        #endregion

        #region ChangeConfiguration

        [Theory]
        [InlineData(TypeReport.Bug, ContextReport.Site)]
        [InlineData(TypeReport.Bug, ContextReport.Question)]
        [InlineData(TypeReport.Improuvment, ContextReport.Site)]
        [InlineData(TypeReport.Improuvment, ContextReport.Question)]
        public void ChangeConfiguration_WithValidParameters_ShouldUpdateConfiguration(
            TypeReport type, ContextReport context)
        {
            // Arrange
            var report = new Report();

            // Act
            report.ChangeConfiguration(type, context);

            // Assert
            Assert.Equal(type, report.Type);
            Assert.Equal(context, report.Context);
        }

        [Fact]
        public void ChangeConfiguration_MultipleChanges_ShouldUpdateCorrectly()
        {
            // Arrange
            var report = new Report();

            // Act
            report.ChangeConfiguration(TypeReport.Bug, ContextReport.Site);
            var firstType = report.Type;
            var firstContext = report.Context;

            report.ChangeConfiguration(TypeReport.Improuvment, ContextReport.Question);

            // Assert
            Assert.Equal(TypeReport.Bug, firstType);
            Assert.Equal(ContextReport.Site, firstContext);
            Assert.Equal(TypeReport.Improuvment, report.Type);
            Assert.Equal(ContextReport.Question, report.Context);
        }

        #endregion

        #region Scénarios d'intégration

        [Fact]
        public void CompleteWorkflow_CreateAndModifyReport_ShouldWorkCorrectly()
        {
            // Arrange & Act
            var report = new Report(
                TypeReport.Bug,
                ContextReport.Site,
                "Problème initial",
                "user@example.com"
            );

            report.ChangeExplanation("Nouvelle explication détaillée");
            report.ChangeMail("newemail@example.com");
            report.ChangeConfiguration(TypeReport.Improuvment, ContextReport.Question);

            // Assert
            Assert.Equal("Nouvelle explication détaillée", report.Explanation);
            Assert.Equal("newemail@example.com", report.Mail);
            Assert.Equal(TypeReport.Improuvment, report.Type);
            Assert.Equal(ContextReport.Question, report.Context);
        }

        [Fact]
        public void Constructor_WithInvalidExplanation_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Report(TypeReport.Bug, ContextReport.Site, "123", null)
            );
        }

        [Fact]
        public void Constructor_WithInvalidMail_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Report(TypeReport.Bug, ContextReport.Site, "Explication valide", "invalid-email")
            );
        }

        #endregion
    }
}
