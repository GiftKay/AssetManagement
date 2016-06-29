﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AssetManagement.Domain.Entities;
using System.Data.Entity.Infrastructure;

namespace AssetManagement.WebUI.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            Domain.Context.AssetManagementEntities  AME = new Domain.Context.AssetManagementEntities();
            
            return View(AME.Invoices.ToList());
        }
        
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Invoice InvoiceModel, HttpPostedFileBase upload)
        { 
            try
            {
                Domain.Context.AssetManagementEntities AME = new Domain.Context.AssetManagementEntities();
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var invoice = new Invoice
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            invoice.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        InvoiceModel.CaptureDate = DateTime.Today;
                        InvoiceModel.Content = invoice.Content;
                        InvoiceModel.ContentType = invoice.ContentType;
                        InvoiceModel.FileName = invoice.FileName;          
                    }
                    AME.Invoices.Add(InvoiceModel);
                    AME.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(InvoiceModel);
        }

        [HttpGet]
        public ActionResult ViewInvoice(int id)
        {
            Domain.Context.AssetManagementEntities AME = new Domain.Context.AssetManagementEntities();
            var file = AME.Invoices.Find(id);
            return File(file.Content, file.ContentType);
        }

    }
}