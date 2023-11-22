using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace UnitTests
{
    public class CardTest : IClassFixture<TestDatabaseFixture>
    {
        public CardTest(TestDatabaseFixture fixture)
            => Fixture = fixture;

        public TestDatabaseFixture Fixture { get; }

        [Fact]
        public async void GetAll()
        {
            using var context = Fixture.CreateContext();
            var service = new CardService(context);

            var result = await service.GetAll(1, "");

            Assert.NotEmpty(result);
        }

        [Fact]
        public async void Creation_succeced()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            var newCard = new Card
            {
                Number = "0000",
                Department = 80,
                Source = 90,
                FactoryNumber = "",
                ProductCode = "",
                InvoiceNumber = "Test",
                Date = DateTime.Now
            };

            await service.Create(newCard);

            int id = newCard.Id;

            context.ChangeTracker.Clear();

            Assert.NotNull(await service.Get(id));
        }

        [Fact]
        public async void Creation_failed_because_not_unique_number()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            var newCard = new Card
            {
                Number = "123/80.19",
                Department = 80,
                Source = 90,
                FactoryNumber = "",
                ProductCode = "",
                InvoiceNumber = "Test",
                Date = DateTime.Now
            };

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.Create(newCard));
        }

        [Fact]
        public async void Creation_failed_because_of_not_unique_act_number()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            var newCard = new Card
            {
                Number = "0000",
                Department = 80,
                Source = 90,
                FactoryNumber = "",
                ProductCode = "",
                InvoiceNumber = "Test",
                Date = DateTime.Now,
                ActNumber = "123/80.19"
            };

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.Create(newCard));
        }

        [Fact]
        public async void Change_succeced()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            var card = await service.Get(9647);

            card.FactoryNumber = "12345";

            await service.Edit(card);

            context.ChangeTracker.Clear();

            card = await service.Get(9647);

            Assert.Equal("12345", card.FactoryNumber);
        }

        [Fact]
        public async void Change_failed_because_of_not_unique_number()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            var card = await service.Get(9647);

            card.Number = "123/80.19";

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.Edit(card));
        }

        [Fact]
        public async void Change_failed_because_of_not_unique_act_number()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            var card = await service.Get(9647);

            card.ActNumber = "123/80.19";

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.Edit(card));
        }

        [Fact]
        public async void Export_to_NormaVremia_failed_because_of_product_code_is_not_repair()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.ExportToNormaVremia(123, 4));
        }

        [Fact]
        public async void Addition_as_extracted_child_card_failed_because_id_and_parentId_are_equal()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.AddExtractedCard(110, 110));
        }

        [Fact]
        public async void Addition_as_installed_child_card_failed_because_id_and_parentId_are_equal()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.AddInstalledCard(110, 110));
        }

        [Fact]
        public async void Addition_as_extracted_child_card_failed_because_parent_card_confirmed_by_ooioit()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.AddExtractedCard(110, 6219));
        }

        [Fact]
        public async void Addition_as_installed_child_card_failed_because_parent_card_confirmed_by_ooioit()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.AddExtractedCard(110, 6219));
        }

        [Fact]
        public async void Addition_as_extracted_child_card_failed_because_parent_card_confirmed()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.AddExtractedCard(110, 989));
        }

        [Fact]
        public async void Addition_as_installed_child_card_failed_because_parent_card_confirmed()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.AddInstalledCard(110, 989));
        }

        [Fact]
        public async void Deleting_of_extracted_child_card_failed_because_parent_card_confirmed_by_ooiot()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.DeleteExtractedCard(6253));
        }

        [Fact]
        public async void Deleting_of_installed_child_card_failed_because_parent_card_confirmed_by_ooiot()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.DeleteInstalledCard(6253));
        }

        [Fact]
        public async void Deleting_of_extracted_child_card_failed_because_parent_card_confirmed()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.DeleteExtractedCard(797));
        }

        [Fact]
        public async void Deleting_of_installed_child_card_failed_because_parent_card_confirmed()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            var service = new CardService(context);

            await Assert.ThrowsAsync<BusinessLogicException>(() => service.DeleteInstalledCard(8318));
        }
    }
}