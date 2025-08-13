using TDDAssignment;
using System;
using NUnit.Framework;
using Moq;
using NSubstitute;
using System.Reflection.PortableExecutable;

[TestFixture]
class OfficeControllerTests
{
    // This test checks if the project template is correctly set up on your machine.
    // Uncomment the test method below and verify that it appears in the Test Explorer (Test -> Test Explorer).
    // Running the unit test should pass with the message "Example Test Passed."
    // If the test is not visible, doesn't run, or fails: ask a tutor for assistance.
    // Once you confirm the template runs unit tests successfully, you can delete this test and comment.

    //----------------------------OFFICE ID TESTS------------------------------------------------
    //L1R1 L1R2
    //makes sure when a constructors created that the office id is set to lowercase
    [TestCase("TestID", "testid")]
    [TestCase("Office123", "office123")]
    [TestCase("LOWERCASE", "lowercase")]
    [Test]
    public void Constructor_ValidOfficeID_SetsLowercaseID(string inputID, string expectedID)
    { 
        //act
        OfficeController controller = new OfficeController(inputID);//calls constructor of the OfficeController class

        //Assert
        Assert.That(controller.GetOfficeID(), Is.EqualTo(expectedID)); 
    }

    //L1R3
    //checks that when setofficeid is called using uppcase that it gets set to lowercase
    [TestCase("NEWID", "newid")]
    [TestCase("OtherID", "otherid")]
    [TestCase("1A", "1a")]
    [Test]
    public void SetOfficeID_UppercaseCharacters_ReturnsLowercase(string newID, string expectedID)
    {
        //arrange 
        OfficeController controller = new OfficeController("InitialID"); //create new instance of officecontroller 

        //act
        controller.SetOfficeID(newID); //calls setofficeid, whicch changes newid to lowercase

        //assert
        Assert.That(controller.GetOfficeID(), Is.EqualTo(expectedID));// checks that the officeID matches expectedID (is lowercase).
    }
    //-------------------------------------------------------------------------------------------

    //----------------------STATE TRANSITION TESTS-----------------------------------------------
    //L1R5 L2R5
    //makes sure that VALID state inputs DO update cureentstate and then return true
    [TestCase("open")]
    [TestCase("closed")]
    [TestCase("out of hours")]
    [Test]
    public void SetCurrentState_ValidState_ReturnsTrue(string validState)
    {
        //arrange
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();//create fake version of doormanager
        stubDoorManager.OpenAllDoors().Returns(true); //stub that calls openalldoors and sets it to being true

        //create new instance of officecontroller, with fake dependencies
        OfficeController controller = new OfficeController("TestID", Substitute.For<ILightManager>(), Substitute.For<IFireAlarmManager>(), stubDoorManager,
                                                            Substitute.For<IWebService>(), Substitute.For<IEmailService>());
        //act
        //tries to set currentstate to a validstate
        bool result = controller.SetCurrentState(validState);

        //assert
        //checks that its true as it should be valid
        Assert.That(result, Is.True);
        Assert.That(controller.GetCurrentState(), Is.EqualTo(validState)); //checks crurentstate was updated to the validstate
    }

    //L1R5
    //makes sure that invalid state inputs DONT update currentstate so they return false
    [TestCase("invalid")]
    [TestCase(" ")]
    [TestCase("12345")]
    [Test]
    public void SetCurrentState_InvalidState_ReturnsFalse(string invalidState)
    {
        //arrange
        OfficeController controller = new OfficeController("TestID"); //creates new instance of officecontroller, sets office id to TestID

        //act
        bool result = controller.SetCurrentState(invalidState); //calls setcurrenstate with INVALIDstate

        //assert
        Assert.That(result, Is.False);//checks that setcurrentstate returned false
        Assert.That(controller.GetCurrentState(), Is.Not.EqualTo(invalidState)); //makes sure currentstate isnt set to an invalid state.
    }

    //L1R7
    //makes sure that setting the state to the same thing again doesnt change it and just returns true
    //checks that if calling setcurrentstate with the SAMESTATE the office is already in
    //doesnt change anything and then returns true
    [TestCase("open")]
    [TestCase("closed")]
    [TestCase("out of hours")]
    [TestCase("fire alarm")]
    [TestCase("fire drill")]
    [Test]
    public void SetCurrentState_TransitionToSameState_ReturnsTrue(string sameState)
    {
        //arrange
        //create fake DoorManager 
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();
        //stub that makes sur when openalldoors is called it returns true
        stubDoorManager.OpenAllDoors().Returns(true);

        //create new instance of office controller, passing in the systems dependencies
        OfficeController controller = new OfficeController("TestID", Substitute.For<ILightManager>(), Substitute.For<IFireAlarmManager>(), stubDoorManager,
                                                            Substitute.For<IWebService>(), Substitute.For<IEmailService>());

        //sets currentstate to samestate (any valid state)
        controller.SetCurrentState(sameState);

        //act
        //set currentstate to the same state again^^
        bool result = controller.SetCurrentState(sameState);

        //assert
        //checks setcurrentstate returned true.
        Assert.That(result, Is.True);
        //makes sure getcurrentstate is set to same state
        Assert.That(controller.GetCurrentState(), Is.EqualTo(sameState));
    }
    //-------------------------------------------------------------------------------------------

    //---------------------------SPECIFIC STATW TRANSITIONS--------------------------------------
    //L1R6
    //makes sure that VALID state transitions work proeprly and return true
    [TestCase("closed", "out of hours", true)]
    [TestCase("out of hours", "open", true)]
    [TestCase("open", "out of hours", true)]
    [Test]
    public void SetCurrentState_ValidTransitions_ReturnsTrue(string initialState, string newState, bool expectedResult)
    {
        //arrange
        //create fake doormanager
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();
        //stub to make sure openalldoors returns true
        stubDoorManager.OpenAllDoors().Returns(true);

        //create new instance of office controller, passing in the systems dependencies
        OfficeController controller = new OfficeController("TestID", Substitute.For<ILightManager>(),
                                                            Substitute.For<IFireAlarmManager>(), stubDoorManager,
                                                            Substitute.For<IWebService>(), Substitute.For<IEmailService>());
        //sets currentstate
        controller.SetCurrentState(initialState);

        //act
        //sets currentstate its moving to
        bool result = controller.SetCurrentState(newState);

        //assert
        //checks to see if newstate is the expectedstate
        Assert.That(result, Is.EqualTo(expectedResult));
        //checks the currentstate was properly updated to the new state
        Assert.That(controller.GetCurrentState(), Is.EqualTo(newState));
    }

    //L1R6
    //makes sure that INVALID state transitions return false and dont change anything
    [TestCase("open", "closed")]
    [TestCase("closed", "open")]
    [Test]
    public void SetCurrentState_InvalidTransitions_ReturnsFalse(string initialState, string invalidState)
    {
        //arrange
        //create fake door manager
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();
        //stub to make sure openalldoors returns true
        stubDoorManager.OpenAllDoors().Returns(true);

        //create new instance of office controller, passing in the systems dependencies
        OfficeController controller = new OfficeController("TestID", Substitute.For<ILightManager>(),
                                                            Substitute.For<IFireAlarmManager>(), stubDoorManager,
                                                            Substitute.For<IWebService>(), Substitute.For<IEmailService>());

        //start at "out of hours"
        controller.SetCurrentState(initialState);

        //act
        //tries to set to invalid state
        bool result = controller.SetCurrentState(invalidState);

        //assert
        //checks the result is false as its invalid
        Assert.That(result, Is.False);
        //checks the currentsate hasnt changed
        Assert.That(controller.GetCurrentState(), Is.EqualTo(initialState));
    }

    //-------------------------CONSTRUCTOR TESTS-------------------------------------------------
    //L2R1
    //makes sure the constructor initialises th office with a valid start state and
    //makes sure the id and state are both lowercase
    [TestCase("MainOffice", "OPEN", "mainoffice", "open")]
    [TestCase("Office123", "CLOSED", "office123", "closed")]
    [TestCase("SecondOffice", "OUT OF HOURS", "secondoffice", "out of hours")]
    [Test]
    public void Constructor_ValidStartState_SetLowercaseIDAndState(string inputID, string inputState, string expectedID, string expectedState)
    {
        //act
        OfficeController controller = new OfficeController(inputID, inputState); //creates new instance of officecontroller, sets office id to inputID and currentstate to inputstate

        //assert
        Assert.That(controller.GetOfficeID(), Is.EqualTo(expectedID)); // checks if the officeid stored in construcor is same as expectedid
        Assert.That(controller.GetCurrentState(), Is.EqualTo(expectedState)); // same here but for the states.
    }

    //--------------------------MOCK OF DEPENDENCIES TESTS---------------------------------------
    //L2R2
    //this checks if constructor properly saves VALID dependencies passed into it
    [Test]
    public void Constructor_ValidDependencies_StoresInterfaces()
    {
        //arrange
        //create fake versions of the sysetms dependencies 
        ILightManager stubLightManager = Substitute.For<ILightManager>();
        IFireAlarmManager stubFireAlarmManager = Substitute.For<IFireAlarmManager>();
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();
        IWebService stubWebService = Substitute.For<IWebService>();
        IEmailService stubEmailService = Substitute.For<IEmailService>();

        //act
        //creates an instance of officecontroller with stub dependencies
        OfficeController controller = new OfficeController("TestID", stubLightManager, stubFireAlarmManager,
                                                                    stubDoorManager, stubWebService,
                                                                    stubEmailService);

        //assert
        //checks the constrcutor properly stores the dependencies
        Assert.That(controller.GetLightManager(), Is.EqualTo(stubLightManager));
        Assert.That(controller.GetFireAlarmManager(), Is.EqualTo(stubFireAlarmManager));
        Assert.That(controller.GetDoorManager(), Is.EqualTo(stubDoorManager));
        Assert.That(controller.GetWebService(), Is.EqualTo(stubWebService));
        Assert.That(controller.GetEmailService(), Is.EqualTo(stubEmailService));
    }

    //L2R3
    //this makes fake versions of the managers, simulates their status then checks to
    //see if they return the same once called.
    [Test]
    public void GetStatus_ReturnsProperStatus_FromManagers()
    {
        //arrange
        //creates fake versions of the mananger dependencies
        ILightManager stubLightManager = Substitute.For<ILightManager>();
        IFireAlarmManager stubFireAlarmManager = Substitute.For<IFireAlarmManager>();
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();

        //simulate the status of system managers
        stubLightManager.GetStatus().Returns    ("Lights, OK, OK, OK, OK, FAULT, OK, OK, OK, FAULT, OK,");
        stubFireAlarmManager.GetStatus().Returns("FireAlarm, OK, OK, OK, FAULT, OK, OK,");
        stubDoorManager.GetStatus().Returns     ("Doors, OK, OK, FAULT, OK, FAULT, OK, OK, OK,");

        //create instance of officecontroller with the stub dependencies
        OfficeController controller = new OfficeController("TestOffice", stubLightManager, stubFireAlarmManager, stubDoorManager,
                                                            Substitute.For<IWebService>(), Substitute.For<IEmailService>());

        string expectedReport = "Lights, OK, OK, OK, OK, FAULT, OK, OK, OK, FAULT, OK," 
                                + "Doors, OK, OK, FAULT, OK, FAULT, OK, OK, OK," 
                                + "FireAlarm, OK, OK, OK, FAULT, OK, OK,";

        //act
        //gets statusreport
        string statusReport = controller.GetStatusReport();

        //assert
        //getStatusReport is called and should be equal to the simulated status of the system managers
        Assert.That(statusReport, Is.EqualTo(expectedReport));
    }

    //L2R4
    //test to check that when state is open, that all the doors then open.
    [Test]
    public void SetCurrentState_SetToOpen_CallOpenAllDoors()
    {
        //arrange
        //create fake version of door manager dependency
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();
        stubDoorManager.OpenAllDoors().Returns(false);//when openalldoors is called, return false

        //create instance of officecontroller with fake dependencies
        OfficeController controller = new OfficeController("TestID", Substitute.For<ILightManager>(), Substitute.For<IFireAlarmManager>(), stubDoorManager,
                                                            Substitute.For<IWebService>(), Substitute.For<IEmailService>());

        //act
        //set currentstate to open
        bool result = controller.SetCurrentState("open");

        //assert
        Assert.That(result, Is.False);//checks setcurrentstate was false
        Assert.That(controller.GetCurrentState(), Is.EqualTo("out of hours"));//checks the state stayed as out of hours.

        //mock that verifies that the openalldoors method was called
        stubDoorManager.Received(1).OpenAllDoors();
    }

    //L3R1
    [Test]
    public void SetCurrentState_ClosedState_CallsLockAllDoorsAndTurnsOffLights()
    {
        //arrange
        //create fake door and lights dependencies
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();
        ILightManager stubLightManager = Substitute.For<ILightManager>();

        //create instance of officecontroller with fake dependencies
        OfficeController controller = new OfficeController("TestID", stubLightManager, Substitute.For<IFireAlarmManager>(), stubDoorManager,
                                                            Substitute.For<IWebService>(), Substitute.For<IEmailService>());

        //act
        //sets currentstate to closed
        bool result = controller.SetCurrentState("closed");

        //assert
        Assert.That(result, Is.True);//checks to make sure setcurrentstate is set to closed
        Assert.That(controller.GetCurrentState(), Is.EqualTo("closed"));//checks currentstate is equal to closed

        //mocks to make sure the 2 methods were called.
        stubDoorManager.Received(1).LockAllDoors();
        stubLightManager.Received(1).SetLights(false);
    }

    //
    [Test]
    public void SetCurrentState_ToFireAlarm_CallsRightFireAlarmActions()
    {
        //arrange
        //fake versions of the system dependencies
        IDoorManager stubDoorManager = Substitute.For<IDoorManager>();
        ILightManager stubLightManager = Substitute.For<ILightManager>();
        IFireAlarmManager stubFireAlarmManager = Substitute.For<IFireAlarmManager>();
        IWebService stubWebService = Substitute.For<IWebService>();

        //creates instance of officecontroller, inject fake dependcnies
        OfficeController controller = new OfficeController("TestID", stubLightManager, stubFireAlarmManager,
                                                                    stubDoorManager, stubWebService,
                                                                    Substitute.For<IEmailService>());

        //act
        bool result = controller.SetCurrentState("fire alarm");

        //assert
        //checks state was actually set
        Assert.That(result, Is.True);
        //checks getcurrentstte is set to fire alarm
        Assert.That(controller.GetCurrentState(), Is.EqualTo("fire alarm"));

        //mocks to track if the methods were properly called
        stubDoorManager.Received(1).OpenAllDoors();
        stubLightManager.Received(1).SetLights(true);
        stubFireAlarmManager.Received(1).SetAlarm(true);
        stubWebService.Received(1).LogFireAlarm("fire alarm");
    }
    //use the following naming convention for your test method names MethodBeingTested_TestScenario_ExpectedOutput
    //E.g. SetCurrentState_InvalidState_ReturnsFalse

    //Write Test Code Here...
}