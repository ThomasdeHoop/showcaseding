describe('Admin Access Test', () => {
    beforeEach(() => {
        // Visit the login page
        cy.visit('https://localhost:7074/Identity/Account/Login');

        // Fill in email and password fields
        cy.get('input[name="Input.Email"]').type('fortnite@fortnite.fortnite4');
        cy.get('input[name="Input.Password"]').type('!1aaaA');

        // Submit the form
        cy.get('form').submit();

        // Check if redirected to the expected URL
        cy.url().should('eq', 'https://localhost:7074/');
    });

    it('Accesses Admin Page after successful login and checks if it says admin', () => {
        // Visit the admin page
        cy.visit('https://localhost:7074/Home/Admin');

        // Check if the page contains "Admin"
        cy.contains('Admin').should('exist');
    });
});

describe('Access Denied Test', () => {
    beforeEach(() => {
        // Visit the login page
        cy.visit('https://localhost:7074/Identity/Account/Login');

        // Fill in email and password fields
        cy.get('input[name="Input.Email"]').type('a@a.a');
        cy.get('input[name="Input.Password"]').type('!1aaaA');

        // Submit the form
        cy.get('form').submit();

        // Check if redirected to the expected URL
        cy.url().should('eq', 'https://localhost:7074/');
    });

    it('Accesses Admin Page with restricted email and checks if it says Access denied', () => {
        // Visit the admin page
        cy.visit('https://localhost:7074/Home/Admin');

        // Check if the page contains "Access denied"
        cy.contains('Access denied').should('exist');
    });
});
