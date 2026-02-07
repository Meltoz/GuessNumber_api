using Domain.ValueObjects;

namespace UnitTests.Domain.ValueObjects
{
    public class UnitTests
    {
        #region Create Tests - Cas Nominaux

        [Fact]
        public void Create_WithValidUnit_ShouldCreateUnit()
        {
            // Arrange
            var unitValue = "kg";

            // Act
            var unit = Unit.Create(unitValue);

            // Assert
            Assert.NotNull(unit);
            Assert.Equal(unitValue, unit.Value);
        }

        [Fact]
        public void Create_WithNullUnit_ShouldCreateUnitWithNull()
        {
            // Arrange
            string unitValue = null;

            // Act
            var unit = Unit.Create(unitValue);

            // Assert
            Assert.NotNull(unit);
            Assert.Null(unit.Value);
        }

        [Fact]
        public void Create_WithEmptyUnit_ShouldCreateUnitWithEmpty()
        {
            // Arrange
            var unitValue = "";

            // Act
            var unit = Unit.Create(unitValue);

            // Assert
            Assert.NotNull(unit);
            Assert.Equal(string.Empty, unit.Value);
        }

        [Fact]
        public void Create_WithWhitespaceUnit_ShouldCreateUnit()
        {
            // Arrange
            var unitValue = "   ";

            // Act
            var unit = Unit.Create(unitValue);

            // Assert
            Assert.NotNull(unit);
            Assert.Equal(unitValue, unit.Value);
        }

        [Fact]
        public void Create_WithMetricUnits_ShouldCreateUnit()
        {
            // Arrange & Act & Assert
            var unit1 = Unit.Create("m");
            Assert.Equal("m", unit1.Value);

            var unit2 = Unit.Create("km");
            Assert.Equal("km", unit2.Value);

            var unit3 = Unit.Create("g");
            Assert.Equal("g", unit3.Value);

            var unit4 = Unit.Create("kg");
            Assert.Equal("kg", unit4.Value);

            var unit5 = Unit.Create("L");
            Assert.Equal("L", unit5.Value);

            var unit6 = Unit.Create("mL");
            Assert.Equal("mL", unit6.Value);
        }

        [Fact]
        public void Create_WithComplexUnits_ShouldCreateUnit()
        {
            // Arrange & Act & Assert
            var unit1 = Unit.Create("m/s");
            Assert.Equal("m/s", unit1.Value);

            var unit2 = Unit.Create("kg/m³");
            Assert.Equal("kg/m³", unit2.Value);

            var unit3 = Unit.Create("m²");
            Assert.Equal("m²", unit3.Value);

            var unit4 = Unit.Create("°C");
            Assert.Equal("°C", unit4.Value);
        }

        [Fact]
        public void Create_WithImperialUnits_ShouldCreateUnit()
        {
            // Arrange & Act & Assert
            var unit1 = Unit.Create("ft");
            Assert.Equal("ft", unit1.Value);

            var unit2 = Unit.Create("lb");
            Assert.Equal("lb", unit2.Value);

            var unit3 = Unit.Create("oz");
            Assert.Equal("oz", unit3.Value);
        }

        [Fact]
        public void Create_WithLongUnitDescription_ShouldCreateUnit()
        {
            // Arrange
            var unitValue = "kilowatt-hours per square meter";

            // Act
            var unit = Unit.Create(unitValue);

            // Assert
            Assert.Equal(unitValue, unit.Value);
        }

        [Fact]
        public void Create_WithSingleCharacter_ShouldCreateUnit()
        {
            // Arrange
            var unitValue = "m";

            // Act
            var unit = Unit.Create(unitValue);

            // Assert
            Assert.Equal(unitValue, unit.Value);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            var unitValue = "kg";
            var unit = Unit.Create(unitValue);

            // Act
            var result = unit.ToString();

            // Assert
            Assert.Equal(unitValue, result);
        }

        [Fact]
        public void ToString_WithNullValue_ShouldReturnNull()
        {
            // Arrange
            var unit = Unit.Create(null);

            // Act
            var result = unit.ToString();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ToString_WithEmptyValue_ShouldReturnEmpty()
        {
            // Arrange
            var unit = Unit.Create("");

            // Act
            var result = unit.ToString();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_WithMatchingSubstring_ShouldReturnTrue()
        {
            // Arrange
            var unit = Unit.Create("kg/m³");

            // Act
            var result = unit.Contains("kg");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithMatchingSubstringDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var unit = Unit.Create("kg/m³");

            // Act
            var result = unit.Contains("KG");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNonMatchingSubstring_ShouldReturnFalse()
        {
            // Arrange
            var unit = Unit.Create("kg");

            // Act
            var result = unit.Contains("lb");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithEmptyString_ShouldReturnTrue()
        {
            // Arrange
            var unit = Unit.Create("kg");

            // Act
            var result = unit.Contains("");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithFullMatch_ShouldReturnTrue()
        {
            // Arrange
            var unit = Unit.Create("kg");

            // Act
            var result = unit.Contains("kg");

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_WithSameUnitValue_ShouldReturnTrue()
        {
            // Arrange
            var unit1 = Unit.Create("kg");
            var unit2 = Unit.Create("kg");

            // Act
            var result = unit1.Equals(unit2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithSameUnitValueDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var unit1 = Unit.Create("kg");
            var unit2 = Unit.Create("KG");

            // Act
            var result = unit1.Equals(unit2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentUnitValues_ShouldReturnFalse()
        {
            // Arrange
            var unit1 = Unit.Create("kg");
            var unit2 = Unit.Create("lb");

            // Act
            var result = unit1.Equals(unit2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var unit = Unit.Create("kg");

            // Act
            var result = unit.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OperatorEquals_WithSameUnitValue_ShouldReturnTrue()
        {
            // Arrange
            var unit1 = Unit.Create("kg");
            var unit2 = Unit.Create("kg");

            // Act
            var result = unit1 == unit2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OperatorNotEquals_WithDifferentUnitValues_ShouldReturnTrue()
        {
            // Arrange
            var unit1 = Unit.Create("kg");
            var unit2 = Unit.Create("lb");

            // Act
            var result = unit1 != unit2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_WithSameUnitValue_ShouldReturnSameHashCode()
        {
            // Arrange
            var unit1 = Unit.Create("kg");
            var unit2 = Unit.Create("kg");

            // Act
            var hash1 = unit1.GetHashCode();
            var hash2 = unit2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithSameUnitValueDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var unit1 = Unit.Create("kg");
            var unit2 = Unit.Create("KG");

            // Act
            var hash1 = unit1.GetHashCode();
            var hash2 = unit2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        #endregion

        #region Implicit Operator Tests

        [Fact]
        public void ImplicitOperator_ShouldConvertToString()
        {
            // Arrange
            var unit = Unit.Create("kg");

            // Act
            string result = unit;

            // Assert
            Assert.Equal("kg", result);
        }

        [Fact]
        public void ImplicitOperator_WithNullValue_ShouldReturnNull()
        {
            // Arrange
            var unit = Unit.Create(null);

            // Act
            string result = unit;

            // Assert
            Assert.Null(result);
        }

        #endregion
    }
}
