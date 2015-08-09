﻿using System;
using NUnit.Framework;
using Moq;
using Services.Interfaces;
using crisicheckinweb.Controllers;
using Models;
using System.Web.Mvc;
using crisicheckinweb.ViewModels;
using Services.Exceptions;
using System.Linq;

namespace WebProjectTests
{
    [TestFixture]
    public class DisasterControllerTests
    {
        private DisasterController _controllerUnderTest;

        private Mock<IDisaster> _disaster;

        private Mock<IDisasterClusterService> _disasterClusterService;

        private Mock<ICluster> _cluster;

        [SetUp]
        public void SetUp()
        {
            _disaster = new Mock<IDisaster>();

            _cluster = new Mock<ICluster>();

            _disasterClusterService = new Mock<IDisasterClusterService>();

            _controllerUnderTest = new DisasterController(_disaster.Object, _cluster.Object, _disasterClusterService.Object);
        }

        [Test]
        public void Assign_ValidDataAdd_ReturnsListView()
        {
            //Arrange
            var DisList = new System.Collections.Generic.List<Disaster>() { new Disaster {Name = "test", Id = 1, IsActive = false } };
            _disaster.Setup(x => x.GetList()).Returns(DisList);
            // Act
            var viewModel = new DisasterViewModel
            {
                Id = -1,
                Name = "test",
                IsActive = false,
                SelectedDisasterClusters = (new System.Collections.Generic.List<SelectedDisasterCluster>() { new SelectedDisasterCluster { Id = 1, Name = "Test", Selected = true }, }),
            };
            var response = _controllerUnderTest.Create(viewModel);

            // Assert
            var result = response as RedirectResult;
            Assert.IsTrue(result.Url.Equals("/Disaster/List"));
        }

        [Test]
        public void Assign_ValidDataUpdate_ReturnsListView()
        {
            // Arrange
            var disClusList = new System.Collections.Generic.List<DisasterCluster>(){new DisasterCluster { Id = -1, ClusterId = 1, DisasterId = 1 }};
            _disasterClusterService.Setup(x => x.GetClustersForADisaster(1)).Returns(disClusList);

            // Act
            var viewModel = new DisasterViewModel
            {
                Id = 1,
                Name = "updated",
                IsActive = true,
                SelectedDisasterClusters = (new System.Collections.Generic.List<SelectedDisasterCluster>(){new SelectedDisasterCluster{Id = 1, Name = "Test", Selected = true},}),
            };
            var response = _controllerUnderTest.Create(viewModel);

            // Assert

            var result = response as RedirectResult;
            Assert.IsTrue(result.Url.Equals("/Disaster/List"));
        }

        [Test]
        public void Assign_duplicateName_ReturnsCreateView()
        {
            // Arrange
            _disaster.Setup(x => x.Create(
                It.IsAny<Disaster>())).Throws(new DisasterAlreadyExistsException());

            // Act
            var viewModel = new DisasterViewModel { Id = -1, Name = "test", IsActive = true };
            var response = _controllerUnderTest.Create(viewModel);

            // Assert
            var view = response as ViewResult;
            Assert.AreEqual("Create", view.ViewName);
            Assert.IsTrue(view.ViewData.ModelState.Count >= 1);
        }
    }
}
