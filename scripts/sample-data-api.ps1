# FHIR-AI Backend Sample Data Generator (PowerShell)
# 
# This script populates the FHIR-AI Backend with realistic healthcare sample data
# using the REST API endpoints. It creates a comprehensive dataset including:
# - Patients with demographic information
# - Observations (vital signs, lab results)
# - Encounters (visits, procedures)
# - Conditions (diagnoses, problems)
# - MedicationRequests (prescriptions)
# 
# Usage:
# 1. Start the FHIR-AI Backend application
# 2. Run: .\scripts\sample-data-api.ps1
# 3. Check the console output for results

param(
    [string]$ApiBaseUrl = "https://localhost:52871",
    [string]$TenantId = "demo-tenant",
    [string]$FhirScopes = "user/* patient/*"
)

# Sample data arrays
$samplePatients = @(
    @{
        resourceType = "Patient"
        name = @(@{ use = "official"; family = "Smith"; given = @("John", "Michael") })
        gender = "male"
        birthDate = "1990-01-15"
        address = @(@{ 
            use = "home"
            type = "physical"
            line = @("123 Main St")
            city = "Springfield"
            state = "IL"
            postalCode = "62701"
            country = "US"
        })
        telecom = @(
            @{ system = "phone"; value = "+1-555-0123"; use = "home" }
            @{ system = "email"; value = "john.smith@email.com"; use = "home" }
        )
        maritalStatus = @{ coding = @(@{ system = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus"; code = "M"; display = "Married" }) }
        contact = @(@{ 
            relationship = @(@{ coding = @(@{ system = "http://terminology.hl7.org/CodeSystem/v2-0131"; code = "C"; display = "Emergency Contact" }) })
            name = @{ use = "official"; family = "Smith"; given = @("Sarah", "Jane") }
            telecom = @(@{ system = "phone"; value = "+1-555-0124"; use = "mobile" })
        })
    },
    @{
        resourceType = "Patient"
        name = @(@{ use = "official"; family = "Johnson"; given = @("Emily", "Rose") })
        gender = "female"
        birthDate = "1985-06-22"
        address = @(@{ 
            use = "home"
            type = "physical"
            line = @("456 Oak Ave")
            city = "Springfield"
            state = "IL"
            postalCode = "62702"
            country = "US"
        })
        telecom = @(
            @{ system = "phone"; value = "+1-555-0125"; use = "mobile" }
            @{ system = "email"; value = "emily.johnson@email.com"; use = "home" }
        )
        maritalStatus = @{ coding = @(@{ system = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus"; code = "S"; display = "Single" }) }
    },
    @{
        resourceType = "Patient"
        name = @(@{ use = "official"; family = "Williams"; given = @("Robert", "David") })
        gender = "male"
        birthDate = "1978-12-03"
        address = @(@{ 
            use = "home"
            type = "physical"
            line = @("789 Pine St")
            city = "Springfield"
            state = "IL"
            postalCode = "62703"
            country = "US"
        })
        telecom = @(
            @{ system = "phone"; value = "+1-555-0126"; use = "work" }
            @{ system = "email"; value = "robert.williams@email.com"; use = "work" }
        )
        maritalStatus = @{ coding = @(@{ system = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus"; code = "M"; display = "Married" }) }
    },
    @{
        resourceType = "Patient"
        name = @(@{ use = "official"; family = "Brown"; given = @("Lisa", "Marie") })
        gender = "female"
        birthDate = "1992-03-18"
        address = @(@{ 
            use = "home"
            type = "physical"
            line = @("321 Elm St")
            city = "Springfield"
            state = "IL"
            postalCode = "62704"
            country = "US"
        })
        telecom = @(
            @{ system = "phone"; value = "+1-555-0127"; use = "mobile" }
            @{ system = "email"; value = "lisa.brown@email.com"; use = "home" }
        )
        maritalStatus = @{ coding = @(@{ system = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus"; code = "D"; display = "Divorced" }) }
    },
    @{
        resourceType = "Patient"
        name = @(@{ use = "official"; family = "Davis"; given = @("Michael", "James") })
        gender = "male"
        birthDate = "1965-09-30"
        address = @(@{ 
            use = "home"
            type = "physical"
            line = @("654 Maple Dr")
            city = "Springfield"
            state = "IL"
            postalCode = "62705"
            country = "US"
        })
        telecom = @(
            @{ system = "phone"; value = "+1-555-0128"; use = "home" }
            @{ system = "email"; value = "michael.davis@email.com"; use = "home" }
        )
        maritalStatus = @{ coding = @(@{ system = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus"; code = "M"; display = "Married" }) }
    }
)

$sampleObservations = @(
    # Blood Pressure
    @{
        resourceType = "Observation"
        status = "final"
        code = @{
            coding = @(@{
                system = "http://loinc.org"
                code = "85354-9"
                display = "Blood pressure panel with all children optional"
            })
        }
        subject = @{ reference = "Patient/" }
        effectiveDateTime = ""
        component = @(
            @{
                code = @{
                    coding = @(@{
                        system = "http://loinc.org"
                        code = "8480-6"
                        display = "Systolic blood pressure"
                    })
                }
                valueQuantity = @{ value = 120; unit = "mmHg"; system = "http://unitsofmeasure.org"; code = "mm[Hg]" }
            },
            @{
                code = @{
                    coding = @(@{
                        system = "http://loinc.org"
                        code = "8462-4"
                        display = "Diastolic blood pressure"
                    })
                }
                valueQuantity = @{ value = 80; unit = "mmHg"; system = "http://unitsofmeasure.org"; code = "mm[Hg]" }
            }
        )
    },
    # Body Weight
    @{
        resourceType = "Observation"
        status = "final"
        code = @{
            coding = @(@{
                system = "http://loinc.org"
                code = "29463-7"
                display = "Body weight"
            })
        }
        subject = @{ reference = "Patient/" }
        effectiveDateTime = ""
        valueQuantity = @{ value = 70; unit = "kg"; system = "http://unitsofmeasure.org"; code = "kg" }
    },
    # Body Height
    @{
        resourceType = "Observation"
        status = "final"
        code = @{
            coding = @(@{
                system = "http://loinc.org"
                code = "8302-2"
                display = "Body height"
            })
        }
        subject = @{ reference = "Patient/" }
        effectiveDateTime = ""
        valueQuantity = @{ value = 175; unit = "cm"; system = "http://unitsofmeasure.org"; code = "cm" }
    },
    # Heart Rate
    @{
        resourceType = "Observation"
        status = "final"
        code = @{
            coding = @(@{
                system = "http://loinc.org"
                code = "8867-4"
                display = "Heart rate"
            })
        }
        subject = @{ reference = "Patient/" }
        effectiveDateTime = ""
        valueQuantity = @{ value = 72; unit = "beats/min"; system = "http://unitsofmeasure.org"; code = "/min" }
    },
    # Temperature
    @{
        resourceType = "Observation"
        status = "final"
        code = @{
            coding = @(@{
                system = "http://loinc.org"
                code = "8310-5"
                display = "Body temperature"
            })
        }
        subject = @{ reference = "Patient/" }
        effectiveDateTime = ""
        valueQuantity = @{ value = 37.0; unit = "C"; system = "http://unitsofmeasure.org"; code = "Cel" }
    }
)

$sampleEncounters = @(
    @{
        resourceType = "Encounter"
        status = "finished"
        class = @{ system = "http://terminology.hl7.org/CodeSystem/v3-ActCode"; code = "AMB"; display = "ambulatory" }
        type = @(@{
            coding = @(@{
                system = "http://snomed.info/sct"
                code = "185389000"
                display = "General medical examination"
            })
        })
        subject = @{ reference = "Patient/" }
        period = @{ start = ""; end = "" }
        reasonCode = @(@{
            coding = @(@{
                system = "http://snomed.info/sct"
                code = "185389000"
                display = "General medical examination"
            })
        })
    },
    @{
        resourceType = "Encounter"
        status = "finished"
        class = @{ system = "http://terminology.hl7.org/CodeSystem/v3-ActCode"; code = "AMB"; display = "ambulatory" }
        type = @(@{
            coding = @(@{
                system = "http://snomed.info/sct"
                code = "185389000"
                display = "General medical examination"
            })
        })
        subject = @{ reference = "Patient/" }
        period = @{ start = ""; end = "" }
        reasonCode = @(@{
            coding = @(@{
                system = "http://snomed.info/sct"
                code = "185389000"
                display = "General medical examination"
            })
        })
    }
)

$sampleConditions = @(
    @{
        resourceType = "Condition"
        clinicalStatus = @{
            coding = @(@{
                system = "http://terminology.hl7.org/CodeSystem/condition-clinical"
                code = "active"
                display = "Active"
            })
        }
        verificationStatus = @{
            coding = @(@{
                system = "http://terminology.hl7.org/CodeSystem/condition-ver-status"
                code = "confirmed"
                display = "Confirmed"
            })
        }
        category = @(@{
            coding = @(@{
                system = "http://terminology.hl7.org/CodeSystem/condition-category"
                code = "problem-list-item"
                display = "Problem List Item"
            })
        })
        code = @{
            coding = @(@{
                system = "http://snomed.info/sct"
                code = "38341003"
                display = "Hypertensive disorder"
            })
        }
        subject = @{ reference = "Patient/" }
        onsetDateTime = ""
        recordedDate = ""
    },
    @{
        resourceType = "Condition"
        clinicalStatus = @{
            coding = @(@{
                system = "http://terminology.hl7.org/CodeSystem/condition-clinical"
                code = "active"
                display = "Active"
            })
        }
        verificationStatus = @{
            coding = @(@{
                system = "http://terminology.hl7.org/CodeSystem/condition-ver-status"
                code = "confirmed"
                display = "Confirmed"
            })
        }
        category = @(@{
            coding = @(@{
                system = "http://terminology.hl7.org/CodeSystem/condition-category"
                code = "problem-list-item"
                display = "Problem List Item"
            })
        })
        code = @{
            coding = @(@{
                system = "http://snomed.info/sct"
                code = "73211009"
                display = "Diabetes mellitus"
            })
        }
        subject = @{ reference = "Patient/" }
        onsetDateTime = ""
        recordedDate = ""
    }
)

$sampleMedicationRequests = @(
    @{
        resourceType = "MedicationRequest"
        status = "active"
        intent = "order"
        medicationCodeableConcept = @{
            coding = @(@{
                system = "http://www.nlm.nih.gov/research/umls/rxnorm"
                code = "197361"
                display = "Lisinopril 10 MG Oral Tablet"
            })
        }
        subject = @{ reference = "Patient/" }
        authoredOn = ""
        requester = @{ reference = "Practitioner/practitioner-001" }
        dosageInstruction = @(@{
            text = "Take 1 tablet daily"
            timing = @{
                repeat = @{
                    frequency = 1
                    period = 1
                    periodUnit = "d"
                }
            }
            route = @{
                coding = @(@{
                    system = "http://snomed.info/sct"
                    code = "26643006"
                    display = "Oral route"
                })
            }
            doseAndRate = @(@{
                type = @{
                    coding = @(@{
                        system = "http://terminology.hl7.org/CodeSystem/dose-rate-type"
                        code = "ordered"
                        display = "Ordered"
                    })
                }
                doseQuantity = @{
                    value = 1
                    unit = "tablet"
                    system = "http://unitsofmeasure.org"
                    code = "{tbl}"
                }
            })
        })
    },
    @{
        resourceType = "MedicationRequest"
        status = "active"
        intent = "order"
        medicationCodeableConcept = @{
            coding = @(@{
                system = "http://www.nlm.nih.gov/research/umls/rxnorm"
                code = "860975"
                display = "Metformin 500 MG Oral Tablet"
            })
        }
        subject = @{ reference = "Patient/" }
        authoredOn = ""
        requester = @{ reference = "Practitioner/practitioner-001" }
        dosageInstruction = @(@{
            text = "Take 1 tablet twice daily with meals"
            timing = @{
                repeat = @{
                    frequency = 2
                    period = 1
                    periodUnit = "d"
                }
            }
            route = @{
                coding = @(@{
                    system = "http://snomed.info/sct"
                    code = "26643006"
                    display = "Oral route"
                })
            }
            doseAndRate = @(@{
                type = @{
                    coding = @(@{
                        system = "http://terminology.hl7.org/CodeSystem/dose-rate-type"
                        code = "ordered"
                        display = "Ordered"
                    })
                }
                doseQuantity = @{
                    value = 1
                    unit = "tablet"
                    system = "http://unitsofmeasure.org"
                    code = "{tbl}"
                }
            })
        })
    }
)

# Utility functions
function Get-RandomDate {
    param(
        [DateTime]$Start,
        [DateTime]$End
    )
    $randomTicks = Get-Random -Minimum $Start.Ticks -Maximum $End.Ticks
    $randomDate = [DateTime]::new($randomTicks)
    return $randomDate.ToString("yyyy-MM-dd")
}

function Get-RandomValue {
    param(
        [double]$Min,
        [double]$Max,
        [int]$Decimals = 0
    )
    $value = Get-Random -Minimum $Min -Maximum $Max
    if ($Decimals -eq 0) {
        return [Math]::Round($value)
    } else {
        return [Math]::Round($value, $Decimals)
    }
}

function Invoke-ApiRequest {
    param(
        [string]$Endpoint,
        [string]$Method = "GET",
        [object]$Body = $null
    )
    
    $url = "$ApiBaseUrl$Endpoint"
    $headers = @{
        "Content-Type" = "application/json"
        "X-Tenant-ID" = $TenantId
        "X-FHIR-Scopes" = $FhirScopes
    }
    
    $params = @{
        Uri = $url
        Method = $Method
        Headers = $headers
    }
    
    if ($Body) {
        $params.Body = $Body | ConvertTo-Json -Depth 10
    }
    
    try {
        $response = Invoke-RestMethod @params
        return $response
    } catch {
        Write-Error "API request failed: $($_.Exception.Message)"
        throw
    }
}

function New-FhirResource {
    param(
        [string]$ResourceType,
        [object]$ResourceData
    )
    
    try {
        $body = @{
            resourceType = $ResourceType
            resourceJson = ($ResourceData | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-ApiRequest -Endpoint "/fhir/$ResourceType" -Method "POST" -Body $body
        Write-Host "✅ Created $ResourceType with ID: $($response.fhirId)" -ForegroundColor Green
        return $response.fhirId
    } catch {
        Write-Host "❌ Failed to create $ResourceType : $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

function Start-SampleDataGeneration {
    Write-Host "🚀 Starting FHIR-AI Backend Sample Data Generation..." -ForegroundColor Cyan
    Write-Host ""
    
    $createdPatients = @()
    $createdObservations = @()
    $createdEncounters = @()
    $createdConditions = @()
    $createdMedicationRequests = @()
    
    # Create Patients
    Write-Host "📋 Creating Patients..." -ForegroundColor Yellow
    foreach ($patientData in $samplePatients) {
        $patientId = New-FhirResource -ResourceType "Patient" -ResourceData $patientData
        if ($patientId) {
            $createdPatients += $patientId
        }
    }
    
    # Create Observations for each patient
    Write-Host "`n🔬 Creating Observations..." -ForegroundColor Yellow
    foreach ($patientId in $createdPatients) {
        foreach ($observationTemplate in $sampleObservations) {
            $observationData = $observationTemplate.Clone()
            $observationData.subject.reference = "Patient/$patientId"
            $observationData.effectiveDateTime = Get-RandomDate -Start ([DateTime]"2023-01-01") -End ([DateTime]::Now)
            
            # Add some variation to the values
            if ($observationData.valueQuantity) {
                $baseValue = $observationData.valueQuantity.value
                $variation = $baseValue * 0.1 # 10% variation
                $observationData.valueQuantity.value = Get-RandomValue -Min ($baseValue - $variation) -Max ($baseValue + $variation) -Decimals 1
            }
            
            if ($observationData.component) {
                foreach ($comp in $observationData.component) {
                    if ($comp.valueQuantity) {
                        $baseValue = $comp.valueQuantity.value
                        $variation = $baseValue * 0.1
                        $comp.valueQuantity.value = Get-RandomValue -Min ($baseValue - $variation) -Max ($baseValue + $variation) -Decimals 1
                    }
                }
            }
            
            $observationId = New-FhirResource -ResourceType "Observation" -ResourceData $observationData
            if ($observationId) {
                $createdObservations += $observationId
            }
        }
    }
    
    # Create Encounters for each patient
    Write-Host "`n🏥 Creating Encounters..." -ForegroundColor Yellow
    foreach ($patientId in $createdPatients) {
        foreach ($encounterTemplate in $sampleEncounters) {
            $startDate = Get-RandomDate -Start ([DateTime]"2023-01-01") -End ([DateTime]::Now)
            $endDate = [DateTime]::Parse($startDate).AddHours((Get-RandomValue -Min 1 -Max 4)) # 1-4 hour encounters
            
            $encounterData = $encounterTemplate.Clone()
            $encounterData.subject.reference = "Patient/$patientId"
            $encounterData.period.start = "$startDate`T09:00:00Z"
            $encounterData.period.end = $endDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
            
            $encounterId = New-FhirResource -ResourceType "Encounter" -ResourceData $encounterData
            if ($encounterId) {
                $createdEncounters += $encounterId
            }
        }
    }
    
    # Create Conditions for some patients
    Write-Host "`n🏥 Creating Conditions..." -ForegroundColor Yellow
    for ($i = 0; $i -lt $createdPatients.Count; $i += 2) { # Every other patient gets a condition
        $patientId = $createdPatients[$i]
        $conditionTemplate = $sampleConditions[$i % $sampleConditions.Count]
        
        $conditionData = $conditionTemplate.Clone()
        $conditionData.subject.reference = "Patient/$patientId"
        $conditionData.onsetDateTime = Get-RandomDate -Start ([DateTime]"2020-01-01") -End ([DateTime]"2023-01-01")
        $conditionData.recordedDate = Get-RandomDate -Start ([DateTime]"2023-01-01") -End ([DateTime]::Now)
        
        $conditionId = New-FhirResource -ResourceType "Condition" -ResourceData $conditionData
        if ($conditionId) {
            $createdConditions += $conditionId
        }
    }
    
    # Create Medication Requests for patients with conditions
    Write-Host "`n💊 Creating Medication Requests..." -ForegroundColor Yellow
    for ($i = 0; $i -lt $createdPatients.Count; $i += 2) {
        $patientId = $createdPatients[$i]
        $medicationTemplate = $sampleMedicationRequests[$i % $sampleMedicationRequests.Count]
        
        $medicationData = $medicationTemplate.Clone()
        $medicationData.subject.reference = "Patient/$patientId"
        $medicationData.authoredOn = Get-RandomDate -Start ([DateTime]"2023-01-01") -End ([DateTime]::Now)
        
        $medicationId = New-FhirResource -ResourceType "MedicationRequest" -ResourceData $medicationData
        if ($medicationId) {
            $createdMedicationRequests += $medicationId
        }
    }
    
    # Summary
    Write-Host "`n📊 Sample Data Generation Complete!" -ForegroundColor Green
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host "✅ Patients created: $($createdPatients.Count)" -ForegroundColor White
    Write-Host "✅ Observations created: $($createdObservations.Count)" -ForegroundColor White
    Write-Host "✅ Encounters created: $($createdEncounters.Count)" -ForegroundColor White
    Write-Host "✅ Conditions created: $($createdConditions.Count)" -ForegroundColor White
    Write-Host "✅ Medication Requests created: $($createdMedicationRequests.Count)" -ForegroundColor White
    Write-Host ""
    $total = $createdPatients.Count + $createdObservations.Count + $createdEncounters.Count + $createdConditions.Count + $createdMedicationRequests.Count
    Write-Host "🎉 Total FHIR resources created: $total" -ForegroundColor Cyan
    
    # Test data retrieval
    Write-Host "`n🔍 Testing data retrieval..." -ForegroundColor Yellow
    try {
        $patients = Invoke-ApiRequest -Endpoint "/fhir/Patient?take=5"
        $patientCount = if ($patients.resourceType -eq "Bundle") { $patients.entry.Count } else { 0 }
        Write-Host "📋 Retrieved $patientCount patients" -ForegroundColor White
        
        $observations = Invoke-ApiRequest -Endpoint "/fhir/Observation?take=5"
        $observationCount = if ($observations.resourceType -eq "Bundle") { $observations.entry.Count } else { 0 }
        Write-Host "🔬 Retrieved $observationCount observations" -ForegroundColor White
    } catch {
        Write-Host "❌ Data retrieval test failed: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Write-Host "`n✨ Sample data generation completed successfully!" -ForegroundColor Green
    Write-Host "You can now explore the data through the Swagger UI or API endpoints." -ForegroundColor White
}

# Run the sample data generation
Start-SampleDataGeneration
