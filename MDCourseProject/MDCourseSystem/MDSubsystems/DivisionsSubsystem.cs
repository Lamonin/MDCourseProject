using System.Collections.Generic;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.MDCourseSystem.MDSubsystems
{
    public class DivisionsSubsystem:ISubsystem
    {
        private int _catalogueIndex;
        private DivisionsCatalogue _divisionsCatalogue;
        private SendRequestsCatalogue _sendRequestsCatalogue;
        
        public DivisionsSubsystem()
        {
            CatalogueIndex = 0;
            _divisionsCatalogue = new DivisionsCatalogue();
            _sendRequestsCatalogue = new SendRequestsCatalogue();
        }

        public void LoadDefaultFirstCatalogue()
        {
            LoadFirstCatalogue("DefaultFiles/divisions_default.txt");
        }
        public void LoadFirstCatalogue(string filePath)
        {
            _divisionsCatalogue.Load(filePath);
        }

        public void LoadDefaultSecondCatalogue()
        {
            LoadSecondCatalogue("DefaultFiles/sendrequests_default.txt");
        }
        public void LoadSecondCatalogue(string filePath)
        {
            _sendRequestsCatalogue.Load(filePath);
        }

        public int CatalogueIndex
        {
            get => _catalogueIndex;
            set
            {
                if (value < 0) value = 0;
                _catalogueIndex = value;

                //Этот вывод в консоль так... для красоты)
                if (_catalogueIndex == 0)
                    MDDebugConsole.WriteLine("Выбран справочник Подразделения");
                else
                    MDDebugConsole.WriteLine("Выбран справочник Отправленные заявки");
            }
        }

        public Catalogue Catalogue => _catalogueIndex == 0 ? _divisionsCatalogue : _sendRequestsCatalogue;

        public IEnumerable<string> CataloguesNames => new[]{_divisionsCatalogue.Name, _sendRequestsCatalogue.Name};
    }
}


