﻿using Codecaine.PeribahasaVector.Domain.Entities;

namespace Codecaine.PeribahasaVector.UnitTests.Domain.Entities
{
    [TestFixture]
    internal class PeribahasaTests
    {
        private const string ValidTeks = "Air dicincang tidak akan putus";
        private const string ValidMaksud = "Perselisihan antara adik-beradik tidak akan berpanjangan";
        private const string ValidTranslation = "Water that is chopped will not break";
        private const string ValidContext = "Used to refer to sibling conflict";
        private const string ValidSource = "Dewan Bahasa dan Pustaka";

        [Test]
        public void Constructor_WithValidValues_ShouldSetProperties()
        {
            var entity = new Peribahasa(ValidTeks, ValidMaksud, ValidTranslation, ValidContext, ValidSource);

            Assert.That(entity.Teks, Is.EqualTo(ValidTeks));
            Assert.That(entity.Maksud, Is.EqualTo(ValidMaksud));
            Assert.That(entity.Translation, Is.EqualTo(ValidTranslation));
            Assert.That(entity.Context, Is.EqualTo(ValidContext));
            Assert.That(entity.Source, Is.EqualTo(ValidSource));
            Assert.That(entity.Content, Is.EqualTo($"{ValidTeks} {ValidMaksud} {ValidTranslation} {ValidContext} {ValidSource}"));
        }

        [Test]
        public void Constructor_WithEmptyTeks_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Peribahasa("", ValidMaksud, ValidTranslation, ValidContext, ValidSource));
            Assert.That(ex.Message, Does.Contain("Teks"));
        }

        [Test]
        public void Constructor_WithEmptyMaksud_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Peribahasa(ValidTeks, "", ValidTranslation, ValidContext, ValidSource));
            Assert.That(ex.Message, Does.Contain("Maksud"));
        }

        [Test]
        public void Constructor_WithEmptyTranslation_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Peribahasa(ValidTeks, ValidMaksud, "", ValidContext, ValidSource));
            Assert.That(ex.Message, Does.Contain("Translation"));
        }

        [Test]
        public void Constructor_WithEmptyContext_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Peribahasa(ValidTeks, ValidMaksud, ValidTranslation, "", ValidSource));
            Assert.That(ex.Message, Does.Contain("Context"));
        }

        [Test]
        public void Constructor_WithEmptySource_ShouldThrow()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Peribahasa(ValidTeks, ValidMaksud, ValidTranslation, ValidContext, ""));
            Assert.That(ex.Message, Does.Contain("Source"));
        }

       
    }
}
