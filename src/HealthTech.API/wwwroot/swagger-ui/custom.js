// Custom JavaScript for FHIR-AI Backend Swagger UI

(function() {
    'use strict';

    // Wait for Swagger UI to load
    function waitForSwaggerUI() {
        if (typeof SwaggerUIBundle !== 'undefined') {
            initializeCustomFeatures();
        } else {
            setTimeout(waitForSwaggerUI, 100);
        }
    }

    function initializeCustomFeatures() {
        console.log('Initializing FHIR-AI Backend Swagger UI custom features...');
        
        // Add FHIR resource type badges
        addFhirResourceBadges();
        
        // Add quick test buttons
        addQuickTestButtons();
        
        // Add FHIR examples
        addFhirExamples();
        
        // Add authentication helper
        addAuthenticationHelper();
        
        // Add response formatter
        addResponseFormatter();
        
        // Add keyboard shortcuts
        addKeyboardShortcuts();
        
        // Add dark mode toggle
        addDarkModeToggle();
    }

    function addFhirResourceBadges() {
        // Add badges to operation descriptions
        const operations = document.querySelectorAll('.opblock-summary-description');
        operations.forEach(operation => {
            const text = operation.textContent;
            if (text.includes('Patient') || text.includes('patient')) {
                addBadge(operation, 'Patient');
            } else if (text.includes('Observation') || text.includes('observation')) {
                addBadge(operation, 'Observation');
            } else if (text.includes('Encounter') || text.includes('encounter')) {
                addBadge(operation, 'Encounter');
            } else if (text.includes('Condition') || text.includes('condition')) {
                addBadge(operation, 'Condition');
            }
        });
    }

    function addBadge(element, resourceType) {
        const badge = document.createElement('span');
        badge.className = 'fhir-resource-badge';
        badge.textContent = resourceType;
        element.appendChild(badge);
    }

    function addQuickTestButtons() {
        // Add quick test buttons for common operations
        const operations = document.querySelectorAll('.opblock');
        operations.forEach(operation => {
            const method = operation.querySelector('.opblock-summary-method');
            const path = operation.querySelector('.opblock-summary-path');
            
            if (method && path) {
                const methodText = method.textContent.trim();
                const pathText = path.textContent.trim();
                
                if (methodText === 'GET' && pathText.includes('/fhir/Patient')) {
                    addQuickTestButton(operation, 'Test with Sample Patient');
                } else if (methodText === 'POST' && pathText.includes('/fhir/Patient')) {
                    addQuickTestButton(operation, 'Create Sample Patient');
                }
            }
        });
    }

    function addQuickTestButton(operation, text) {
        const button = document.createElement('button');
        button.className = 'btn try-out__btn';
        button.textContent = text;
        button.style.marginLeft = '10px';
        button.onclick = function() {
            fillSampleData(operation);
        };
        
        const summary = operation.querySelector('.opblock-summary');
        if (summary) {
            summary.appendChild(button);
        }
    }

    function fillSampleData(operation) {
        const method = operation.querySelector('.opblock-summary-method');
        if (method && method.textContent.trim() === 'POST') {
            // Fill sample Patient data
            const textarea = operation.querySelector('textarea');
            if (textarea) {
                textarea.value = JSON.stringify({
                    "resourceType": "Patient",
                    "name": [
                        {
                            "use": "official",
                            "family": "Smith",
                            "given": ["John", "Michael"]
                        }
                    ],
                    "gender": "male",
                    "birthDate": "1990-01-15",
                    "address": [
                        {
                            "use": "home",
                            "type": "physical",
                            "line": ["123 Main St", "Apt 4B"],
                            "city": "Anytown",
                            "state": "CA",
                            "postalCode": "12345",
                            "country": "US"
                        }
                    ]
                }, null, 2);
            }
        }
    }

    function addFhirExamples() {
        // Add FHIR examples to the info section
        const infoSection = document.querySelector('.info');
        if (infoSection) {
            const examplesPanel = document.createElement('div');
            examplesPanel.className = 'fhir-info-panel';
            examplesPanel.innerHTML = `
                <h3>📋 FHIR Resource Examples</h3>
                <p><strong>Patient:</strong> Demographic and administrative information</p>
                <p><strong>Observation:</strong> Clinical measurements and simple assertions</p>
                <p><strong>Encounter:</strong> An interaction between a patient and healthcare provider</p>
                <p><strong>Condition:</strong> Detailed information about conditions, problems, or diagnoses</p>
                <p><strong>MedicationRequest:</strong> An order or request for medication</p>
                <p><strong>Procedure:</strong> An action that is performed on or for a patient</p>
            `;
            infoSection.appendChild(examplesPanel);
        }
    }

    function addAuthenticationHelper() {
        // Add authentication helper to the top
        const topbar = document.querySelector('.topbar');
        if (topbar) {
            const authHelper = document.createElement('div');
            authHelper.style.cssText = 'position: absolute; top: 10px; right: 20px; background: rgba(255,255,255,0.9); padding: 10px; border-radius: 4px; font-size: 12px;';
            authHelper.innerHTML = `
                <strong>🔑 Auth Helper:</strong><br>
                <button onclick="setAuthHeaders()" style="margin: 5px; padding: 5px 10px; background: #4990e2; color: white; border: none; border-radius: 3px; cursor: pointer;">Set Demo Headers</button>
                <button onclick="clearAuthHeaders()" style="margin: 5px; padding: 5px 10px; background: #f93e3e; color: white; border: none; border-radius: 3px; cursor: pointer;">Clear Headers</button>
            `;
            topbar.appendChild(authHelper);
        }
    }

    function addResponseFormatter() {
        // Add response formatter
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.type === 'childList') {
                    const responses = document.querySelectorAll('.responses-table .response');
                    responses.forEach(response => {
                        const responseBody = response.querySelector('.response-col_description');
                        if (responseBody && !responseBody.dataset.formatted) {
                            responseBody.dataset.formatted = 'true';
                            addFormatButton(responseBody);
                        }
                    });
                }
            });
        });
        
        observer.observe(document.body, { childList: true, subtree: true });
    }

    function addFormatButton(responseBody) {
        const formatButton = document.createElement('button');
        formatButton.textContent = 'Format JSON';
        formatButton.style.cssText = 'margin-left: 10px; padding: 2px 8px; background: #4990e2; color: white; border: none; border-radius: 3px; cursor: pointer; font-size: 11px;';
        formatButton.onclick = function() {
            const pre = responseBody.querySelector('pre');
            if (pre) {
                try {
                    const json = JSON.parse(pre.textContent);
                    pre.textContent = JSON.stringify(json, null, 2);
                } catch (e) {
                    console.log('Not valid JSON to format');
                }
            }
        };
        responseBody.appendChild(formatButton);
    }

    function addKeyboardShortcuts() {
        // Add keyboard shortcuts
        document.addEventListener('keydown', function(e) {
            // Ctrl/Cmd + Enter to execute
            if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
                const executeButton = document.querySelector('.execute-wrapper .btn.execute');
                if (executeButton) {
                    executeButton.click();
                }
            }
            
            // Ctrl/Cmd + K to focus search
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                const searchInput = document.querySelector('.filter input');
                if (searchInput) {
                    searchInput.focus();
                }
            }
        });
    }

    function addDarkModeToggle() {
        // Add dark mode toggle
        const topbar = document.querySelector('.topbar');
        if (topbar) {
            const darkModeToggle = document.createElement('button');
            darkModeToggle.innerHTML = '🌙';
            darkModeToggle.style.cssText = 'position: absolute; top: 10px; right: 200px; background: rgba(255,255,255,0.9); border: none; border-radius: 50%; width: 30px; height: 30px; cursor: pointer; font-size: 14px;';
            darkModeToggle.onclick = function() {
                document.body.classList.toggle('dark-mode');
                this.innerHTML = document.body.classList.contains('dark-mode') ? '☀️' : '🌙';
            };
            topbar.appendChild(darkModeToggle);
        }
    }

    // Global functions for auth helper
    window.setAuthHeaders = function() {
        const authInput = document.querySelector('.auth-wrapper input[placeholder*="api_key"]');
        if (authInput) {
            authInput.value = 'demo-api-key-12345';
        }
        
        // Set tenant header
        const tenantInput = document.querySelector('input[placeholder*="X-Tenant-ID"]');
        if (tenantInput) {
            tenantInput.value = 'demo-tenant';
        }
        
        alert('Demo headers set! You can now test the API.');
    };

    window.clearAuthHeaders = function() {
        const inputs = document.querySelectorAll('.auth-wrapper input, input[placeholder*="X-Tenant-ID"]');
        inputs.forEach(input => {
            input.value = '';
        });
        
        alert('Headers cleared!');
    };

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', waitForSwaggerUI);
    } else {
        waitForSwaggerUI();
    }

})();
