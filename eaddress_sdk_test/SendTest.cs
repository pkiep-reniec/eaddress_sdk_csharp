﻿using eaddress_sdk_csharp;
using eaddress_sdk_csharp.common;
using eaddress_sdk_csharp.dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace eaddress_sdk_test
{
    [TestClass]
    public class SendTest
    {
        private ReniecEAddressClient reniecEAddressClient;

        [TestMethod]
        public void SendSingleTest()
        {
            Before();

            List<Attachment> attachments = new List<Attachment>();
            attachments.Add(new Attachment("Archivo demo 1", "https://www.gob.pe/859-plataforma-de-autenticacion-id-peru"));
            attachments.Add(new Attachment("Archivo demo 2", "https://www.gob.pe/859-plataforma-de-autenticacion-id-peru"));
            attachments.Add(new Attachment("Archivo demo 3", "https://www.gob.pe/859-plataforma-de-autenticacion-id-peru"));

            Message oMessage = new Message();

            //oMessage.docType = Constants.TYPE_DOC_RUC;
            //oMessage.doc = "20100055237";
            //oMessage.rep = "46256479,70273865";

            oMessage.docType = Constants.TYPE_DOC_DNI;
            oMessage.doc = "70273865";

            oMessage.subject = "mensaje de prueba individual 222";
            oMessage.message = "<p>Hola <b>Mundo</b></p>";
            oMessage.tag = "tag";

            Task<ApiResponse> result = reniecEAddressClient.SendSingleNotification(oMessage, attachments);
            result.Wait();

            Console.WriteLine(JsonConvert.SerializeObject(result.Result));
            
            Assert.AreEqual(true, result.Result.success);
        }

        [TestMethod]
        public void SendMassiveTest()
        {
            Before();

            FileStream fileCSV = new FileStream(@"..\..\resources\massive.csv", FileMode.Open, FileAccess.Read);

            Message oMessage = new Message();
            oMessage.subject = "22 mensaje de prueba masiva para [[nombres]]";
            oMessage.message = "<p></p>" +
                               "<p>[[nombres]]</p>" +
                               "<p>[[numero_orden]]</p>";
            oMessage.tag = "tag";

            Task<ApiResponse> result = reniecEAddressClient.SendMassiveNotification(oMessage, fileCSV);
            result.Wait();

            Console.WriteLine(result.Result.success);

            //Assert.AreEqual(true, result.Result.success);
        }

        public void Before()
        {
            ConfigAga oConfigAga = new ConfigAga();
            oConfigAga.agaUri = "";
            oConfigAga.timestamp = "";
            oConfigAga.certificateId = "";
            oConfigAga.secretPassword = "";

            reniecEAddressClient = new ReniecEAddressClient(@"..\..\resources\config.json", oConfigAga);
        }
    }
}
