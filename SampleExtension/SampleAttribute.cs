// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System.ComponentModel.DataAnnotations;

namespace SampleExtension
{
    /// <summary>
    /// Binding attribute to place on user code for WebJobs. 
    /// </summary>
    [Binding]
    public class SampleAttribute : Attribute
    {
        // Name of file to read. 
        [AutoResolve]
        [RegularExpression("^[A-Za-z][A-Za-z0-9]{2,128}$")]
        public string FileName { get; set; }

        // path where 
        [AppSetting(Default = "SamplePath")]
        public string Root { get; set; }
    }
}