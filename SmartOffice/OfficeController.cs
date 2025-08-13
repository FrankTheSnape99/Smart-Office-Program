using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDAssignment
{
    public class OfficeController
    {
        //storing office info
        private string officeID;
        private string currentState;
        private string historyState; //used to remember state before "fire alarm/firedrill" 

        //stores the interfaces for all parts of thr office
        private readonly ILightManager lightManager;
        private readonly IFireAlarmManager fireAlarmManager;
        private readonly IDoorManager doorManager;
        private readonly IWebService webService;
        private readonly IEmailService emailService;

        //L1R1
        //constructor that sets a default state and sets id to lowercase
        public OfficeController(string id) 
        {
            officeID = id.ToLower(); //converts id to lowercase //L1R2
            currentState = "out of hours"; //L1R4
            historyState = currentState;

            //null! to stop warnings
            //they are set in the construcor that actually uses them.
            lightManager = null!;
            fireAlarmManager = null!;
            doorManager = null!;
            emailService = null!;
            webService = null!;
        }

        //L2R1
        //constructor that allows specific startstates
        public OfficeController(string id, string startState)
        {
            //converts both to lowercase
            officeID = id.ToLower();
            startState = startState.ToLower();

            //null! to stop warnings
            //they are set in the construcor that actually uses them.
            lightManager = null!;
            fireAlarmManager = null!;
            doorManager = null!;
            emailService = null!;
            webService = null!;

            //array of allowed starting states
            string[] validStartingStates = { "closed", "out of hours", "open" };

            //if the state isnt a start state it gives error.
            if (!validStartingStates.Contains(startState))
            {
                throw new ArgumentException("Argument Exception: OfficeController can only be initialised to the \r\nfollowing states 'open', 'closed', 'out of hours'");
            }
            //set both of thesd to startstate so
            //it starts in a known state AND has history
            currentState = startState;
            historyState = startState;
        }

        //L2R2
        //constructor that passes in dependencies instead of creating them
        public OfficeController(string id, ILightManager iLightManager,
                                IFireAlarmManager iFireAlarmManager, IDoorManager iDoorManager,
                                IWebService iWebService, IEmailService iEmailService)
        {
            officeID = id.ToLower();
            currentState = "out of hours";
            historyState = currentState;

            //stores dependencies for the office systems
            lightManager = iLightManager;
            fireAlarmManager = iFireAlarmManager;
            doorManager = iDoorManager;
            webService = iWebService;
            emailService = iEmailService;
        }

        //getter method, to GET the private>(officeID)
        public string GetOfficeID()
        {
            return officeID;
        }

        //getter method, allows access to currentState
        //returns whatever the state is currently in
        public string GetCurrentState()
        {
            return currentState;
        }

        //setter method, give officeid a new value and sets it to lowercase
        public void SetOfficeID(string newID)
        {
            officeID = newID.ToLower(); //L1R3
        }

        //sets the offices state IF allowed and valid
        //L1R5
        public bool SetCurrentState(string nextState)
        {
            nextState = nextState.ToLower(); //converts newstate to lowercase

            //all valid system states
            string[] validStates = { "closed", "out of hours", "open", "fire drill", "fire alarm"};

            //L1R7 
            //checks if current state of office is the same as the next state.
            if (currentState == nextState)
            {
                return true; //if it is, return true, (cos no change)
            }

            //L1R5
            //only allow nextstate if its valid
            if(!validStates.Contains(nextState))
            {
                return false;
            }

            //L1R6
            //checks the transition to see if allowed, (based on the system transition rules).
            if (!IsValidTransition(currentState, nextState))
            {
                return false; //RETUNR false if nextstate is not valid
            }

            //if currently in either of these, then the NEXT state after them should ho bavk to previous state (history state)
            if (currentState == "fire alarm" || currentState == "fire drill")
            {
                nextState = historyState;
            }

            //if next state is either of these, then should remeber the currentstate
            if (nextState == "fire alarm" || nextState == "fire drill")
            {
                historyState = currentState;
            }

            //if transitioning to open, openalldoors called, if fails, return false
            if (nextState == "open")
            {
                if (!doorManager.OpenAllDoors())
                {
                    return false;
                }
            }

            //if next is closed, lockdoors, turn off lights.
            if (nextState == "closed")
            {
                doorManager.LockAllDoors();//L3R1
                lightManager.SetLights(false); //L3R1 
            }

            //L3R2
            //if next is fire alarm
            //sound alarm, open doors, turn lights on, logs fire alarm online
            if (nextState == "fire alarm")
            {
                fireAlarmManager.SetAlarm(true);
                doorManager.OpenAllDoors();
                lightManager.SetLights(true);
                webService.LogFireAlarm("fire alarm");
            }

            //actually changes the state
            currentState = nextState;
            return true; //confirms the state change was successful
        }
        
        //this method check if the state transitions are valid depending on where its coming from.
        private bool IsValidTransition(string current, string next)
        {
            //L1R6 
            //makes sure that the program follows the diagram.
            switch (current)
            {
                case "open":
                    return next == "out of hours" || next == "fire alarm" || next == "fire drill";
                case "out of hours":
                    return next == "open" || next == "closed" || next == "fire alarm" || next == "fire drill";
                case "closed":
                    return next == "out of hours" || next == "fire alarm" || next == "fire drill";
                case "fire drill":
                case "fire alarm":
                    return next == historyState; // these both return to previous state they were in.
                default:
                    return false;
            }
        }

        //L2R3
        //gets the status of all 3 manager classes
        public string GetStatusReport()
        {
            //getstatus for each manager THEN combines them to make the single string
            return lightManager.GetStatus() + doorManager.GetStatus() + fireAlarmManager.GetStatus();
        }

        //public getters for each of the systems dependencies
        public ILightManager GetLightManager() => lightManager;
        public IFireAlarmManager GetFireAlarmManager() => fireAlarmManager;
        public IDoorManager GetDoorManager() => doorManager;
        public IWebService GetWebService() => webService;
        public IEmailService GetEmailService() => emailService;
        //Write OfficeController code here...
    }
}