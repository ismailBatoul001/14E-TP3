using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model;

namespace Locomotiv.Utils.Services
{
    public class TrainService : ITrainService
    {
        private readonly ITrainRepository _trainRepository;
        private readonly IStationDAL _stationRepository;

        public TrainService(ITrainRepository pTrainRepository, IStationDAL stationRepository)
        {
            _trainRepository = pTrainRepository;
            _stationRepository = stationRepository;
        }

        public void AjouterTrain(string numero, TypeTrain type, EtatTrain etat, int capacite, int stationId)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new Exception("Le numéro du train est requis.");

            if (numero.Length < 3)
                throw new Exception("Le numéro doit contenir au moins 3 caractères.");

            if (capacite <= 0)
                throw new Exception("La capacité doit être positive.");

            int nbTrains = _trainRepository.CountTrainsStation(stationId);
            var station = _stationRepository.GetById(stationId);

            if (station == null)
                throw new Exception("Station invalide.");

            if (nbTrains >= station.CapaciteMaximale)
                throw new Exception("La station est pleine.");

            if (_trainRepository.GetAll().Any(train => train.Numero == numero))
                throw new Exception("Un train avec ce numéro existe déjà.");

            var train = new Train
            {
                Numero = numero,
                Type = type,
                Etat = etat,
                Capacite = capacite,
                StationActuelleId = stationId
            };

            _trainRepository.Add(train);
        }

        public IEnumerable<Train> GetAll()
        {
            return _trainRepository.GetAll();
        }

        public void SupprimerTrain(Train newTrain)
        {
            if (newTrain == null)
            {
                throw new Exception("Le train n'existe pas");
            }
            else
            {
                _trainRepository.Delete(newTrain);
            }
            
        }

        public void ModifierTrain(Train newTrain)
        {
            _trainRepository.Update(newTrain);
        }

        public Train? GetById(int id)
        {
            var train = _trainRepository.GetById(id);
            if(train == null)
            {
                throw new Exception("Train introuvable.");
            }
            return train;
        }

        public IList<Train> GetByStation(int stationId)
        {
            return _trainRepository.GetByStation(stationId);
        }

        public int CompterTrainsStation(int stationId)
        {
            return _trainRepository.CountTrainsStation(stationId);
        }
    }
}
