﻿using DemoApp.Web.DomainModels;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NUnit.Framework;
using Zephyr.Initialization;
using Zephyr.Web.Mvc.Initialization;

namespace DbTests
{
    [TestFixture]
    public class NHibTests
    {
        private readonly IAppBootstrapper _appBootstrapper = new MvcAppBootstrapper();        

        [SetUp]
        public void Init()
        {
            _appBootstrapper.Run();
        }

        [TearDown]
        public void EndCase()
        {
            _appBootstrapper.Dispose();
        }

        [Test]
        public void LoadData()
        {            
            var pubOrelly = new Publisher();
            var pubWrox = new Publisher();
            var pubManning = new Publisher();

            pubOrelly.PublisherName = "O' Reilly";
            pubWrox.PublisherName = "Wrox";
            pubManning.PublisherName = "Manning";

            Book bookOrelly1=new Book();
            bookOrelly1.BookName = "APIs: A Strategy Guide";
            bookOrelly1.Publisher = pubOrelly;
            bookOrelly1.BookDescription = @"Programmers used to be the only people excited about APIs, but now a growing number of companies see them as a hot new product channel. This concise guide describes the tremendous business potential of APIs, and demonstrates how you can use them to provide valuable services to clients, partners, or the public via the Internet. You’ll learn all the steps necessary for building a cohesive API business strategy from experts in the trenches.
                        Facebook and Twitter APIs continue to be extremely successful, and many other companies find that API demand greatly exceeds website traffic.";
            Book bookWrox1 = new Book();
            bookWrox1.BookName = "Professional Microsoft Azure";
            bookWrox1.Publisher = pubWrox;
            bookWrox1.BookDescription = @"Microsoft cloud platform Azure with Visual Studio 2010.";
            Book bookWrox2 = new Book();
            bookWrox2.BookName = "ASP.NET Pro 2";
            bookWrox2.Publisher = pubWrox;
            bookWrox2.BookDescription = @"Sample book desc. This book is for all from beginner to professional.";
            Book bookManning1 = new Book();
            bookManning1.BookName = "Clojure in Action";
            bookManning1.Publisher = pubManning;
            bookManning1.BookDescription = @"Google provided language Clojure comprehensive guide.";
            Book bookManning2 = new Book();
            bookManning2.BookName = "Software Architecture, 2nd Edition";
            bookManning2.Publisher = pubManning;
            bookManning2.BookDescription = @"Big description here.";
            Book bookManning3 = new Book();
            bookManning3.BookName = "NHibernate in Action";
            bookManning3.Publisher = pubManning;
            bookManning3.BookDescription = @"NHIbernate ORM description for the book.";
            
            pubOrelly.Books.Add(bookOrelly1);
            pubWrox.Books.Add(bookWrox1);
            pubWrox.Books.Add(bookWrox2);
            pubManning.Books.Add(bookManning1);
            pubManning.Books.Add(bookManning2);
            pubManning.Books.Add(bookManning3);

            //change tenantid for manning
            //pubManning.TenantId = 1;

            var session = ServiceLocator.Current.GetInstance<ISession>();
            using (var tx=session.BeginTransaction())
            {
                session.SaveOrUpdate(pubOrelly);
                session.SaveOrUpdate(pubWrox);
                session.SaveOrUpdate(pubManning);

                var bob = new User("bob", "password");
                var jeff = new User("jeff", "password");

                session.SaveOrUpdate(bob);
                session.SaveOrUpdate(jeff);

                tx.Commit();
            } 
        }

        [Test]
        public void DeleteOrphan()
        {
            var session = ServiceLocator.Current.GetInstance<ISession>();

            var pub = session.Get<Publisher>(3L);
            var book = session.Get<Book>(4L);
            
            Assert.IsTrue(pub.Books.Contains(book));
            
            pub.Books.Remove(book);

            session.SaveOrUpdate(pub);
            session.Flush();
        }

        [Test]
        public void LazyLoad()
        {
            var session = ServiceLocator.Current.GetInstance<ISession>();

            var pub = session.Get<Publisher>(3L);

            Assert.IsFalse(NHibernateUtil.IsInitialized(pub.Books));
        }

        [Test]
        public void IdentityScope()
        {
            var session = ServiceLocator.Current.GetInstance<ISession>();
            
            var pub1 = session.Get<Publisher>(3L);
            session.Clear();
            session = ServiceLocator.Current.GetInstance<ISession>();
            var pub2 = session.Get<Publisher>(3L);

            Assert.IsTrue(pub1==pub2);
            Assert.IsTrue(pub1.Equals(pub2));
        }


        [Test]
        public void NPlus1()
        {
            var session = ServiceLocator.Current.GetInstance<ISession>(); 

            var pubs = session.CreateCriteria<Publisher>().List<Publisher>();
            int totalBooks = 0;
            foreach (var publisher in pubs)
            {
                foreach (var book in publisher.Books)
                {
                    string name = book.BookName;
                    totalBooks += 1;
                }
            }

            Assert.AreEqual(4, totalBooks);
        }
    }
}
