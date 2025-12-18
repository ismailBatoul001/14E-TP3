using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services;
using Moq;
using Xunit;

namespace LocomotivTests.Data.Repositories
{
    public class InspectionServiceTests
    {
        [Fact]
        public void CreerInspection_UserEstMecanicien_Accepter()
        {
            var userDal = new Mock<IUserDAL>();
            var inspectionDal = new Mock<IInspectionDAL>();

            userDal.Setup(d => d.GetById(10))
                   .Returns(new User { Id = 10, Role = Role.Mecanicien });

            Inspection? inspectionAjoutee = null;
            inspectionDal.Setup(d => d.Add(It.IsAny<Inspection>()))
                         .Callback<Inspection>(i => inspectionAjoutee = i);

            var service = new InspectionService(inspectionDal.Object, userDal.Object);

            var insp = service.CreerInspection(
                trainId: 1,
                mecanicienId: 10,
                type: TypeInspection.Visuelle,
                resultat: ResultatInspection.Conforme,
                observations: "OK"
            );

            inspectionDal.Verify(d => d.Add(It.IsAny<Inspection>()), Times.Once);
            Assert.NotNull(inspectionAjoutee);
            Assert.Equal(1, inspectionAjoutee!.TrainId);
            Assert.Equal(10, inspectionAjoutee.MecanicienId);
            Assert.Equal(Role.Mecanicien, userDal.Object.GetById(10)!.Role);
        }

        [Fact]
        public void CreerInspection_UserNonMecanicien_Refuser()
        {
            var userDal = new Mock<IUserDAL>();
            var inspectionDal = new Mock<IInspectionDAL>();

            userDal.Setup(d => d.GetById(20))
                   .Returns(new User { Id = 20, Role = Role.Employe });

            var service = new InspectionService(inspectionDal.Object, userDal.Object);

            Assert.Throws<UnauthorizedAccessException>(() =>
                service.CreerInspection(
                    trainId: 1,
                    mecanicienId: 20,
                    type: TypeInspection.Visuelle,
                    resultat: ResultatInspection.Conforme,
                    observations: "Devrait échouer"
                )
            );

            inspectionDal.Verify(d => d.Add(It.IsAny<Inspection>()), Times.Never);
        }

        [Fact]
        public void CreerInspection_MecanicienIntrouvable_Refuser()
        {
            var userDal = new Mock<IUserDAL>();
            var inspectionDal = new Mock<IInspectionDAL>();

            userDal.Setup(d => d.GetById(999)).Returns((User?)null);

            var service = new InspectionService(inspectionDal.Object, userDal.Object);

            Assert.Throws<InvalidOperationException>(() =>
                service.CreerInspection(
                    trainId: 1,
                    mecanicienId: 999,
                    type: TypeInspection.Visuelle,
                    resultat: ResultatInspection.Conforme,
                    observations: "Devrait échouer"
                )
            );

            inspectionDal.Verify(d => d.Add(It.IsAny<Inspection>()), Times.Never);
        }
    }
}
