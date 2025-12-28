CREATE OR REPLACE FUNCTION prevent_negative_quantity()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.Quantity < 0 THEN
        RAISE EXCEPTION 'Book quantity cannot be negative';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_prevent_negative_quantity
BEFORE UPDATE OF Quantity
ON Book
FOR EACH ROW
EXECUTE FUNCTION prevent_negative_quantity();

CREATE OR REPLACE FUNCTION auto_order_on_threshold()
RETURNS TRIGGER AS $$
DECLARE
    order_quantity INT := 50;
    admin_username VARCHAR(50) := 'system'; -- Default fallback
BEGIN
    -- Only trigger when quantity drops from above threshold to below threshold
    IF NEW.Quantity < NEW.Threshold AND OLD.Quantity >= OLD.Threshold THEN
        -- Try to get admin username from session setting, fallback to 'system' if not set
        BEGIN
            admin_username := current_setting('myapp.current_user', true);
            -- If setting exists but is empty, use 'system'
            IF admin_username IS NULL OR admin_username = '' THEN
                admin_username := 'system';
            END IF;
        EXCEPTION WHEN OTHERS THEN
            -- Setting doesn't exist, use 'system'
            admin_username := 'system';
        END;

        -- Create admin order with the determined username
        INSERT INTO AdminOrder(OrderDate, "Status", TotalPrice, PubID, CustName)
        VALUES (CURRENT_DATE, 'Pending', order_quantity * NEW.SellingPrice, NEW.PubID, admin_username);

        -- Create admin order item
        INSERT INTO AdminOrderItem(ISBN, OrderNum, Quantity, UnitPrice)
        VALUES (NEW.ISBN, currval(pg_get_serial_sequence('AdminOrder','OrderID')), order_quantity, NEW.SellingPrice);

    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_auto_order_on_threshold
AFTER UPDATE OF Quantity
ON Book
FOR EACH ROW
EXECUTE FUNCTION auto_order_on_threshold();